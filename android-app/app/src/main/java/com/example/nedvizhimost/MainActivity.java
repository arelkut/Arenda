package com.example.nedvizhimost;

import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.nedvizhimost.adapters.PropertyAdapter;
import com.example.nedvizhimost.api.ApiClient;
import com.example.nedvizhimost.models.Property;
import com.google.android.material.bottomnavigation.BottomNavigationView;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity implements PropertyAdapter.OnPropertyClickListener {

    private RecyclerView rvProperties;
    private PropertyAdapter adapter;
    private BottomNavigationView bottomNavigation;
    private Button btnLogin;
    private EditText etSearch;
    private boolean isLoggedIn = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        rvProperties = findViewById(R.id.rvProperties);
        bottomNavigation = findViewById(R.id.bottomNavigation);
        btnLogin = findViewById(R.id.btnLogin);
        etSearch = findViewById(R.id.etSearch);

        // Настройка RecyclerView
        adapter = new PropertyAdapter(this, this);
        rvProperties.setLayoutManager(new LinearLayoutManager(this));
        rvProperties.setAdapter(adapter);

        // Загрузка данных
        loadProperties();

        // Кнопка входа
        btnLogin.setOnClickListener(v -> {
            if (isLoggedIn) {
                // Выход
                ApiClient.setAuthToken(null);
                isLoggedIn = false;
                btnLogin.setText(R.string.login);
                Toast.makeText(this, "Вы вышли из аккаунта", Toast.LENGTH_SHORT).show();
            } else {
                startActivity(new Intent(this, LoginActivity.class));
            }
        });

        // Поиск
        findViewById(R.id.btnSearch).setOnClickListener(v -> {
            String query = etSearch.getText().toString().trim();
            Toast.makeText(this, "Поиск: " + query, Toast.LENGTH_SHORT).show();
            // Здесь будет логика поиска
        });

        // Нижняя навигация
        bottomNavigation.setOnItemSelectedListener(this::handleNavigation);
        
        // Проверка авторизации
        checkAuth();
    }

    @Override
    protected void onResume() {
        super.onResume();
        checkAuth();
    }

    private void checkAuth() {
        String token = ApiClient.getAuthToken();
        if (token != null && !token.isEmpty()) {
            isLoggedIn = true;
            btnLogin.setText(R.string.logout);
        } else {
            isLoggedIn = false;
            btnLogin.setText(R.string.login);
        }
    }

    private void loadProperties() {
        ApiClient.getService().getProperties().enqueue(new Callback<List<Property>>() {
            @Override
            public void onResponse(Call<List<Property>> call, Response<List<Property>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    adapter.setProperties(response.body());
                } else {
                    // Заглушка с тестовыми данными
                    loadMockData();
                }
            }

            @Override
            public void onFailure(Call<List<Property>> call, Throwable t) {
                // Заглушка с тестовыми данными при ошибке
                loadMockData();
            }
        });
    }

    private void loadMockData() {
        List<Property> mockProperties = List.of(
            new Property(1, "2-комнатная квартира", "г. Москва, ул. Тверская, д. 1", 15000000, 
                         "Просторная квартира в центре", "", 2, 54, 5),
            new Property(2, "1-комнатная квартира", "г. Москва, ул. Арбат, д. 10", 12000000, 
                         "Уютная квартира near метро", "", 1, 40, 3),
            new Property(3, "3-комнатная квартира", "г. Москва, Кутузовский пр., д. 25", 25000000, 
                         "Элитная квартира с видом", "", 3, 85, 12)
        );
        adapter.setProperties(mockProperties);
    }

    private boolean handleNavigation(@NonNull MenuItem item) {
        int itemId = item.getItemId();
        
        if (itemId == R.id.nav_catalog) {
            // Уже на главной
            return true;
        } else if (itemId == R.id.nav_favorites) {
            startActivity(new Intent(this, FavoritesActivity.class));
            return true;
        } else if (itemId == R.id.nav_notifications) {
            startActivity(new Intent(this, NotificationsActivity.class));
            return true;
        } else if (itemId == R.id.nav_profile) {
            startActivity(new Intent(this, ProfileActivity.class));
            return true;
        }
        
        return false;
    }

    @Override
    public void onPropertyClick(Property property) {
        // Переход к деталям
        Intent intent = new Intent(this, PropertyDetailActivity.class);
        intent.putExtra("property_id", property.getId());
        startActivity(intent);
    }

    @Override
    public void onFavoriteClick(Property property, int position) {
        if (!isLoggedIn) {
            Toast.makeText(this, "Войдите чтобы добавить в избранное", Toast.LENGTH_SHORT).show();
            return;
        }
        
        // Toggle favorite
        property.setFavorite(!property.isFavorite());
        adapter.notifyItemChanged(position);
        
        Toast.makeText(this, 
            property.isFavorite() ? "Добавлено в избранное" : "Удалено из избранного", 
            Toast.LENGTH_SHORT).show();
    }
}
