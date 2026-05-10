package com.example.nedvizhimost;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.nedvizhimost.api.ApiClient;
import com.example.nedvizhimost.models.AuthResponse;
import com.example.nedvizhimost.models.LoginRequest;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {

    private EditText etEmail, etPassword;
    private Button btnLogin;
    private TextView tvForgotPassword, tvRegister;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        etEmail = findViewById(R.id.etEmail);
        etPassword = findViewById(R.id.etPassword);
        btnLogin = findViewById(R.id.btnLogin);
        tvForgotPassword = findViewById(R.id.tvForgotPassword);
        tvRegister = findViewById(R.id.tvRegister);

        // Кнопка назад
        findViewById(R.id.btnBack).setOnClickListener(v -> finish());

        // Вход
        btnLogin.setOnClickListener(v -> performLogin());

        // Переход к регистрации
        tvRegister.setOnClickListener(v -> {
            startActivity(new Intent(LoginActivity.this, RegisterActivity.class));
        });

        // Забыли пароль (заглушка)
        tvForgotPassword.setOnClickListener(v -> 
            Toast.makeText(this, "Функция восстановления пароля в разработке", Toast.LENGTH_SHORT).show()
        );
    }

    private void performLogin() {
        String email = etEmail.getText().toString().trim();
        String password = etPassword.getText().toString().trim();

        if (email.isEmpty()) {
            etEmail.setError("Введите email");
            return;
        }

        if (password.isEmpty()) {
            etPassword.setError("Введите пароль");
            return;
        }

        btnLogin.setEnabled(false);
        btnLogin.setText(R.string.loading);

        LoginRequest request = new LoginRequest(email, password);
        
        ApiClient.getService().login(request).enqueue(new Callback<AuthResponse>() {
            @Override
            public void onResponse(Call<AuthResponse> call, Response<AuthResponse> response) {
                btnLogin.setEnabled(true);
                btnLogin.setText(R.string.login);

                if (response.isSuccessful() && response.body() != null) {
                    AuthResponse authResponse = response.body();
                    ApiClient.setAuthToken(authResponse.getAccessToken());
                    
                    Toast.makeText(LoginActivity.this, "Вход выполнен успешно", Toast.LENGTH_SHORT).show();
                    
                    // Переход на главный экран
                    Intent intent = new Intent(LoginActivity.this, MainActivity.class);
                    intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
                    startActivity(intent);
                    finish();
                } else {
                    Toast.makeText(LoginActivity.this, "Неверный email или пароль", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<AuthResponse> call, Throwable t) {
                btnLogin.setEnabled(true);
                btnLogin.setText(R.string.login);
                Toast.makeText(LoginActivity.this, "Ошибка: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
