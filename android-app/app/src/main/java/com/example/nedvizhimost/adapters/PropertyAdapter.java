package com.example.nedvizhimost.adapters;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.nedvizhimost.R;
import com.example.nedvizhimost.models.Property;

import java.util.ArrayList;
import java.util.List;

public class PropertyAdapter extends RecyclerView.Adapter<PropertyAdapter.PropertyViewHolder> {

    private Context context;
    private List<Property> propertyList;
    private OnPropertyClickListener listener;

    public interface OnPropertyClickListener {
        void onPropertyClick(Property property);
        void onFavoriteClick(Property property, int position);
    }

    public PropertyAdapter(Context context, OnPropertyClickListener listener) {
        this.context = context;
        this.propertyList = new ArrayList<>();
        this.listener = listener;
    }

    public void setProperties(List<Property> properties) {
        this.propertyList = properties;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public PropertyViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_property, parent, false);
        return new PropertyViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull PropertyViewHolder holder, int position) {
        Property property = propertyList.get(position);
        
        holder.tvPrice.setText(property.getFormattedPrice());
        holder.tvAddress.setText(property.getAddress());
        holder.tvDescription.setText(property.getShortDescription());
        
        // Загрузка изображения
        if (property.getImageUrl() != null && !property.getImageUrl().isEmpty()) {
            Glide.with(context)
                    .load(property.getImageUrl())
                    .placeholder(R.drawable.input_background)
                    .error(R.drawable.input_background)
                    .into(holder.ivPropertyImage);
        }
        
        // Обновление состояния избранного
        updateFavoriteIcon(holder.btnFavorite, property.isFavorite());
        
        // Клик по карточке
        holder.itemView.setOnClickListener(v -> {
            if (listener != null) {
                listener.onPropertyClick(property);
            }
        });
        
        // Клик по избранному
        holder.btnFavorite.setOnClickListener(v -> {
            if (listener != null) {
                listener.onFavoriteClick(property, position);
            }
        });
    }

    private void updateFavoriteIcon(ImageButton btn, boolean isFavorite) {
        if (isFavorite) {
            btn.setImageResource(android.R.drawable.btn_star_big_on);
        } else {
            btn.setImageResource(android.R.drawable.btn_star_big_off);
        }
    }

    @Override
    public int getItemCount() {
        return propertyList.size();
    }

    static class PropertyViewHolder extends RecyclerView.ViewHolder {
        ImageView ivPropertyImage;
        TextView tvPrice, tvAddress, tvDescription;
        ImageButton btnFavorite;

        public PropertyViewHolder(@NonNull View itemView) {
            super(itemView);
            ivPropertyImage = itemView.findViewById(R.id.ivPropertyImage);
            tvPrice = itemView.findViewById(R.id.tvPrice);
            tvAddress = itemView.findViewById(R.id.tvAddress);
            tvDescription = itemView.findViewById(R.id.tvDescription);
            btnFavorite = itemView.findViewById(R.id.btnFavorite);
        }
    }
}
