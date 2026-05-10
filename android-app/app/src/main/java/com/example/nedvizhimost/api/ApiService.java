package com.example.nedvizhimost.api;

import com.example.nedvizhimost.models.*;
import java.util.List;
import retrofit2.Call;
import retrofit2.http.*;

public interface ApiService {
    
    @GET("properties")
    Call<List<Property>> getProperties();
    
    @GET("properties/{id}")
    Call<Property> getPropertyById(@Path("id") int id);
    
    @POST("auth/login")
    Call<AuthResponse> login(@Body LoginRequest request);
    
    @POST("auth/register")
    Call<AuthResponse> register(@Body RegisterRequest request);
    
    @GET("auth/me")
    Call<User> getCurrentUser(@Header("Authorization") String token);
    
    @POST("properties/{id}/favorite")
    Call<Void> toggleFavorite(@Path("id") int id, @Header("Authorization") String token);
}
