package com.example.nedvizhimost.models;

import com.google.gson.annotations.SerializedName;

public class AuthResponse {
    @SerializedName("access_token")
    private String accessToken;
    
    @SerializedName("token_type")
    private String tokenType;
    
    @SerializedName("user")
    private User user;

    public AuthResponse() {}

    public String getAccessToken() { return accessToken; }
    public void setAccessToken(String accessToken) { this.accessToken = accessToken; }
    
    public String getTokenType() { return tokenType; }
    public void setTokenType(String tokenType) { this.tokenType = tokenType; }
    
    public User getUser() { return user; }
    public void setUser(User user) { this.user = user; }
}
