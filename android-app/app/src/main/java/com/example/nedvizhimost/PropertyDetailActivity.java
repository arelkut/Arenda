package com.example.nedvizhimost;

import android.os.Bundle;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.bumptech.glide.Glide;
import com.example.nedvizhimost.models.Property;

public class PropertyDetailActivity extends AppCompatActivity {

    private ImageView ivPropertyImage;
    private TextView tvPrice, tvAddress, tvDescription, tvRooms, tvArea, tvFloor;
    private Button btnFavorite, btnContact;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_property_detail);

        ivPropertyImage = findViewById(R.id.ivPropertyImage);
        tvPrice = findViewById(R.id.tvPrice);
        tvAddress = findViewById(R.id.tvAddress);
        tvDescription = findViewById(R.id.tvDescription);
        tvRooms = findViewById(R.id.tvRooms);
        tvArea = findViewById(R.id.tvArea);
        tvFloor = findViewById(R.id.tvFloor);
        btnFavorite = findViewById(R.id.btnFavorite);
        btnContact = findViewById(R.id.btnContact);

        // Кнопка назад
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // Получение данных
        int propertyId = getIntent().getIntExtra("property_id", 0);
        
        // Загрузка данных (заглушка)
        loadPropertyData(propertyId);

        // Кнопка избранного
        btnFavorite.setOnClickListener(v -> {
            Toast.makeText(this, "Добавлено в избранное", Toast.LENGTH_SHORT).show();
            btnFavorite.setText("В избранном");
        });

        // Кнопка связи
        btnContact.setOnClickListener(v -> {
            Toast.makeText(this, "Открываем связь с агентом...", Toast.LENGTH_SHORT).show();
        });
    }

    private void loadPropertyData(int propertyId) {
        // Заглушка с тестовыми данными
        Property property = new Property(
            propertyId,
            "2-комнатная квартира",
            "г. Москва, ул. Тверская, д. 1",
            15000000,
            "Просторная светлая квартира в центре Москвы. Рядом метро, магазины, школы. Отличный ремонт, мебель и техника. Подходит для семьи или сдачи в аренду.",
            "",
            2,
            54,
            5
        );

        tvPrice.setText(property.getFormattedPrice());
        tvAddress.setText(property.getAddress());
        tvDescription.setText(property.getDescription());
        tvRooms.setText(String.valueOf(property.getRooms()));
        tvArea.setText(String.valueOf((int) property.getArea()));
        tvFloor.setText(String.valueOf(property.getFloor()));

        // Загрузка изображения (заглушка)
        if (property.getImageUrl() != null && !property.getImageUrl().isEmpty()) {
            Glide.with(this)
                    .load(property.getImageUrl())
                    .into(ivPropertyImage);
        } else {
            ivPropertyImage.setBackgroundColor(getColor(android.R.color.darker_gray));
        }
    }
}
