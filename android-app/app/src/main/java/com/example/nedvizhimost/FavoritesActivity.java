package com.example.nedvizhimost;

import android.os.Bundle;
import android.view.MenuItem;
import android.widget.ImageButton;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.nedvizhimost.adapters.PropertyAdapter;
import com.example.nedvizhimost.models.Property;
import com.google.android.material.bottomnavigation.BottomNavigationView;

import java.util.ArrayList;
import java.util.List;

public class FavoritesActivity extends AppCompatActivity implements PropertyAdapter.OnPropertyClickListener {

    private RecyclerView rvFavorites;
    private PropertyAdapter adapter;
    private BottomNavigationView bottomNavigation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_favorites);

        rvFavorites = findViewById(R.id.rvFavorites);
        bottomNavigation = findViewById(R.id.bottomNavigation);

        // Настройка RecyclerView
        adapter = new PropertyAdapter(this, this);
        rvFavorites.setLayoutManager(new LinearLayoutManager(this));
        rvFavorites.setAdapter(adapter);

        // Загрузка избранных (заглушка)
        loadFavorites();

        // Кнопка назад
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // Нижняя навигация
        bottomNavigation.setOnItemSelectedListener(this::handleNavigation);
        bottomNavigation.setSelectedItemId(R.id.nav_favorites);
    }

    private void loadFavorites() {
        // Заглушка с тестовыми данными
        List<Property> favorites = new ArrayList<>();
        Property p1 = new Property(1, "2-комнатная квартира", "г. Москва, ул. Тверская, д. 1", 15000000, 
                     "Просторная квартира в центре", "", 2, 54, 5);
        p1.setFavorite(true);
        favorites.add(p1);
        
        adapter.setProperties(favorites);
    }

    private boolean handleNavigation(@NonNull MenuItem item) {
        int itemId = item.getItemId();
        
        if (itemId == R.id.nav_catalog) {
            startActivity(new Intent(this, MainActivity.class));
            finish();
            return true;
        } else if (itemId == R.id.nav_favorites) {
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
    }

    @Override
    public void onFavoriteClick(Property property, int position) {
        property.setFavorite(!property.isFavorite());
        adapter.notifyItemChanged(position);
        Toast.makeText(this, "Удалено из избранного", Toast.LENGTH_SHORT).show();
    }
}
