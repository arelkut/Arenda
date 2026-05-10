package com.example.nedvizhimost;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.nedvizhimost.api.ApiClient;
import com.example.nedvizhimost.models.AuthResponse;
import com.example.nedvizhimost.models.RegisterRequest;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class RegisterActivity extends AppCompatActivity {

    private EditText etName, etEmail, etPhone, etPassword, etConfirmPassword;
    private Button btnRegister;
    private TextView tvLogin;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_register);

        etName = findViewById(R.id.etName);
        etEmail = findViewById(R.id.etEmail);
        etPhone = findViewById(R.id.etPhone);
        etPassword = findViewById(R.id.etPassword);
        etConfirmPassword = findViewById(R.id.etConfirmPassword);
        btnRegister = findViewById(R.id.btnRegister);
        tvLogin = findViewById(R.id.tvLogin);

        // Кнопка назад
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // Регистрация
        btnRegister.setOnClickListener(v -> performRegister());

        // Переход ко входу
        tvLogin.setOnClickListener(v -> finish());
    }

    private void performRegister() {
        String name = etName.getText().toString().trim();
        String email = etEmail.getText().toString().trim();
        String phone = etPhone.getText().toString().trim();
        String password = etPassword.getText().toString().trim();
        String confirmPassword = etConfirmPassword.getText().toString().trim();

        if (name.isEmpty()) {
            etName.setError("Введите имя");
            return;
        }

        if (email.isEmpty()) {
            etEmail.setError("Введите email");
            return;
        }

        if (phone.isEmpty()) {
            etPhone.setError("Введите телефон");
            return;
        }

        if (password.isEmpty()) {
            etPassword.setError("Введите пароль");
            return;
        }

        if (password.length() < 6) {
            etPassword.setError("Пароль должен быть не менее 6 символов");
            return;
        }

        if (!password.equals(confirmPassword)) {
            etConfirmPassword.setError("Пароли не совпадают");
            return;
        }

        btnRegister.setEnabled(false);
        btnRegister.setText(R.string.loading);

        RegisterRequest request = new RegisterRequest(email, password, name, phone);
        
        ApiClient.getService().register(request).enqueue(new Callback<AuthResponse>() {
            @Override
            public void onResponse(Call<AuthResponse> call, Response<AuthResponse> response) {
                btnRegister.setEnabled(true);
                btnRegister.setText(R.string.register);

                if (response.isSuccessful() && response.body() != null) {
                    AuthResponse authResponse = response.body();
                    ApiClient.setAuthToken(authResponse.getAccessToken());
                    
                    Toast.makeText(RegisterActivity.this, "Регистрация успешна", Toast.LENGTH_SHORT).show();
                    
                    // Переход на главный экран
                    Intent intent = new Intent(RegisterActivity.this, MainActivity.class);
                    intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                    startActivity(intent);
                    finish();
                } else {
                    Toast.makeText(RegisterActivity.this, "Ошибка регистрации", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<AuthResponse> call, Throwable t) {
                btnRegister.setEnabled(true);
                btnRegister.setText(R.string.register);
                Toast.makeText(RegisterActivity.this, "Ошибка: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
