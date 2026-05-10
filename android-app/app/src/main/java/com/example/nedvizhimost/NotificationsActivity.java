package com.example.nedvizhimost;

import android.os.Bundle;
import android.view.MenuItem;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.bottomnavigation.BottomNavigationView;

public class NotificationsActivity extends AppCompatActivity {

    private RecyclerView rvNotifications;
    private BottomNavigationView bottomNavigation;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_notifications);

        rvNotifications = findViewById(R.id.rvNotifications);
        bottomNavigation = findViewById(R.id.bottomNavigation);

        // Настройка RecyclerView
        rvNotifications.setLayoutManager(new LinearLayoutManager(this));

        // Кнопка назад
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // Нижняя навигация
        bottomNavigation.setOnItemSelectedListener(this::handleNavigation);
        bottomNavigation.setSelectedItemId(R.id.nav_notifications);
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
            return true;
        } else if (itemId == R.id.nav_profile) {
            startActivity(new Intent(this, ProfileActivity.class));
            return true;
        }
        
        return false;
    }
}
