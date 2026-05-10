package com.example.nedvizhimost.models;

import com.google.gson.annotations.SerializedName;

public class Property {
    @SerializedName("id")
    private int id;
    
    @SerializedName("title")
    private String title;
    
    @SerializedName("address")
    private String address;
    
    @SerializedName("price")
    private double price;
    
    @SerializedName("description")
    private String description;
    
    @SerializedName("image_url")
    private String imageUrl;
    
    @SerializedName("rooms")
    private int rooms;
    
    @SerializedName("area")
    private double area;
    
    @SerializedName("floor")
    private int floor;
    
    @SerializedName("is_favorite")
    private boolean isFavorite;

    public Property() {}

    public Property(int id, String title, String address, double price, String description, 
                    String imageUrl, int rooms, double area, int floor) {
        this.id = id;
        this.title = title;
        this.address = address;
        this.price = price;
        this.description = description;
        this.imageUrl = imageUrl;
        this.rooms = rooms;
        this.area = area;
        this.floor = floor;
        this.isFavorite = false;
    }

    // Getters and Setters
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }
    
    public String getTitle() { return title; }
    public void setTitle(String title) { this.title = title; }
    
    public String getAddress() { return address; }
    public void setAddress(String address) { this.address = address; }
    
    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }
    
    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }
    
    public String getImageUrl() { return imageUrl; }
    public void setImageUrl(String imageUrl) { this.imageUrl = imageUrl; }
    
    public int getRooms() { return rooms; }
    public void setRooms(int rooms) { this.rooms = rooms; }
    
    public double getArea() { return area; }
    public void setArea(double area) { this.area = area; }
    
    public int getFloor() { return floor; }
    public void setFloor(int floor) { this.floor = floor; }
    
    public boolean isFavorite() { return isFavorite; }
    public void setFavorite(boolean favorite) { isFavorite = favorite; }
    
    public String getFormattedPrice() {
        return String.format("%,d ₽", (int) price);
    }
    
    public String getShortDescription() {
        return rooms + "-комн. квартира, " + (int) area + " м², " + floor + " этаж";
    }
}
