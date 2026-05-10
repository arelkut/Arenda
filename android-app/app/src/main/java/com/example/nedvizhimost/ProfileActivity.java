package com.example.nedvizhimost;

import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.bottomnavigation.BottomNavigationView;

public class ProfileActivity extends AppCompatActivity {

    private TextView tvUserName, tvUserEmail;
    private Button btnLogout;
    private BottomNavigationView bottomNavigation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_profile);

        tvUserName = findViewById(R.id.tvUserName);
        tvUserEmail = findViewById(R.id.tvUserEmail);
        btnLogout = findViewById(R.id.btnLogout);
        bottomNavigation = findViewById(R.id.bottomNavigation);

        // Загрузка данных пользователя (заглушка)
        loadUserProfile();

        // Кнопка назад
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // Выход
        btnLogout.setOnClickListener(v -> {
            ApiClient.setAuthToken(null);
            Toast.makeText(this, "Вы вышли из аккаунта", Toast.LENGTH_SHORT).show();
            finish();
        });

        // Редактирование профиля (заглушка)
        findViewById(R.id.btnEditProfile).setOnClickListener(v -> 
            Toast.makeText(this, "Функция в разработке", Toast.LENGTH_SHORT).show()
        );

        // Мои объявления (заглушка)
        findViewById(R.id.btnMyProperties).setOnClickListener(v -> 
            Toast.makeText(this, "Функция в разработке", Toast.LENGTH_SHORT).show()
        );

        // Настройки (заглушка)
        findViewById(R.id.btnSettings).setOnClickListener(v -> 
            Toast.makeText(this, "Функция в разработке", Toast.LENGTH_SHORT).show()
        );

        // Нижняя навигация
        bottomNavigation.setOnItemSelectedListener(this::handleNavigation);
        bottomNavigation.setSelectedItemId(R.id.nav_profile);
    }

    private void loadUserProfile() {
        // Заглушка - в реальности загружать из API
        tvUserName.setText("Иван Иванов");
        tvUserEmail.setText("ivan@example.com");
    }

    private boolean handleNavigation(@NonNull MenuItem item) {
        int itemId = item.getItemId();
        
        if (itemId == R.id.nav_catalog) {
            startActivity(new Intent(this, MainActivity.class));
            finish();
            return true;
        } else if (itemId == R.id.nav_favorites) {
            startActivity(new Intent(this, FavoritesActivity.class));
            return true;
        } else if (itemId == R.id.nav_notifications) {
            startActivity(new Intent(this, NotificationsActivity.class));
            return true;
        } else if (itemId == R.id.nav_profile) {
            return true;
        }
        
        return false;
    }
}
