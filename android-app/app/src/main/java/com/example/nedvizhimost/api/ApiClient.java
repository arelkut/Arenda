package com.example.nedvizhimost.api;

import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

import java.util.concurrent.TimeUnit;

public class ApiClient {
    private static final String BASE_URL = "http://10.0.2.2:8000/api/"; // Для эмулятора Android
    // Для реального устройства используйте IP вашего компьютера: "http://192.168.x.x:8000/api/"
    
    private static Retrofit retrofit = null;
    private static String authToken = null;

    public static Retrofit getClient() {
        if (retrofit == null) {
            HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
            logging.setLevel(HttpLoggingInterceptor.Level.BODY);

            OkHttpClient.Builder httpClient = new OkHttpClient.Builder()
                    .connectTimeout(30, TimeUnit.SECONDS)
                    .readTimeout(30, TimeUnit.SECONDS)
                    .writeTimeout(30, TimeUnit.SECONDS)
                    .addInterceptor(logging)
                    .addInterceptor(chain -> {
                        okhttp3.Request original = chain.request();
                        okhttp3.Request.Builder requestBuilder = original.newBuilder();
                        
                        if (authToken != null) {
                            requestBuilder.addHeader("Authorization", "Bearer " + authToken);
                        }
                        
                        requestBuilder.method(original.method(), original.body());
                        return chain.proceed(requestBuilder.build());
                    });

            retrofit = new Retrofit.Builder()
                    .baseUrl(BASE_URL)
                    .client(httpClient.build())
                    .addConverterFactory(GsonConverterFactory.create())
                    .build();
        }
        return retrofit;
    }

    public static void setAuthToken(String token) {
        authToken = token;
        // Сбрасываем retrofit чтобы применить новый токен
        retrofit = null;
    }

    public static String getAuthToken() {
        return authToken;
    }

    public static ApiService getService() {
        return getClient().create(ApiService.class);
    }
}
