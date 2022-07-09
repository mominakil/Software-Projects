package com.example.finalassignment;

import androidx.fragment.app.FragmentActivity;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;
import com.example.finalassignment.databinding.ActivityMapsBinding;

public class MapsActivity extends FragmentActivity implements OnMapReadyCallback {

    //Declaring global variables
    private GoogleMap mMap;
    private ActivityMapsBinding binding;

    private Marker CurrentMarker;
    private MarkerOptions MarkerOpts;

    private static final String TAG = "MapsActivity";
    private BroadcastReceiver NotificationReceiver;

    LatLng FirebaseMessageCoordinate;
    double Longitude;
    double Latitude;
    String NotificationMessageBody;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        //Binding and inflating the layout of Maps Activity
        binding = ActivityMapsBinding.inflate(getLayoutInflater());
        setContentView(binding.getRoot());

        //Registering the app with notification channel with the system by passing an instance of NotificationChannel to createNotificationChannel()
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            // Create channel to show notifications.
            String channelId  = getString(R.string.default_notification_channel_id);
            String channelName = getString(R.string.default_notification_channel_name);
            NotificationManager notificationManager =
                    getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(new NotificationChannel(channelId,
                    channelName, NotificationManager.IMPORTANCE_LOW));
        }



        //This will Execute When App is in the Foreground
        //Listening for Broadcast
        NotificationReceiver = new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                //Processed when the data is received from Firebase
                Log.d(TAG, "Called from Broadcast Receiver: (data may follow)");

                //The if statement is processed when the broadcast receiver has been registered with the intent of NotificationReceived name.
                if(intent.getAction().equals("NotificationReceived")){
                    if(intent.getExtras() != null){
                        for(String key : intent.getExtras().keySet()){
                            Object value = intent.getExtras().get(key);
                            Log.d(TAG, "Key: " + key + " Value: " + value);
                        }

                        //Extracting the intent into the Bundle object.
                        Bundle NotificationDataBundle = intent.getExtras();

                        //Extracting the Bundle
                        Longitude = Double.parseDouble(NotificationDataBundle.getString("lng"));
                        Latitude = Double.parseDouble(NotificationDataBundle.getString("lat"));
                        NotificationMessageBody = NotificationDataBundle.getString("messageBody");

                        //Storing the extracted Latitude and Longitude into LatLng Object variable
                        FirebaseMessageCoordinate = new LatLng(Latitude, Longitude);

                        //Change the marker of the map and set title of marker
                        CurrentMarker.setPosition(FirebaseMessageCoordinate);
                        CurrentMarker.setTitle(NotificationMessageBody);

                        //Force the marker to show info window
                        CurrentMarker.showInfoWindow();

                        //Change the camera focus to the new latitude longitude location
                        mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(FirebaseMessageCoordinate, 15.2f));

                    }
                }

            }
        };

    }


    @Override
    protected void onResume() {
        super.onResume();
        Log.d(TAG, "Called from OnResume: (data follows)");

        //Registering the Broadcast receiver
        LocalBroadcastManager.getInstance(this).registerReceiver(NotificationReceiver, new IntentFilter("NotificationReceived"));

        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);

    }

    @Override
    protected void onPause() {
        //Unregistering the Broadcast receiver
        LocalBroadcastManager.getInstance(this).unregisterReceiver(NotificationReceiver);

        super.onPause();
    }


    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    @Override
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap; // Saving the GoogleMap Object to global variable mMap

        // Add a marker in Sydney and move the camera
        LatLng SaskPolytechRegina = new LatLng(50.408189, -104.583234);

        //Initializing the MarkerOptions object to the Saskatchewan Polytechnic Regina location
        MarkerOpts = new MarkerOptions()
                .position(SaskPolytechRegina)
                .title("Marker at SaskPolytech Regina");

        //Adding the marker using MarkerOptions object variable
        CurrentMarker = mMap.addMarker(MarkerOpts);

        //Forcing the marker to display info window
        CurrentMarker.showInfoWindow();

        //Change the camera focus to the new latitude longitude location
        mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(SaskPolytechRegina, 15.2f));

        //calling the method to get the intent received from notification through Firebase when the app is in the background.
        getNotificationIntent();
    }


    public void getNotificationIntent(){
        // This will execute when the app is in the background
        // If a notification message is tapped, any data accompanying the notification
        // message is available in the intent extras. In this sample the launcher
        // intent is fired when the notification is tapped, so any accompanying data would
        // be handled here. If you want a different intent fired, set the click_action
        // field of the notification message to the desired intent. The launcher intent
        // is used when no click_action is specified.
        // Handle possible data accompanying notification message.

        if (getIntent().getExtras() != null) {

            //Logging the Key Value pair by looping through getIntent keySet
            for (String key : getIntent().getExtras().keySet()) {
                Object value = getIntent().getExtras().get(key);
                Log.d(TAG, "Key: " + key + " Value: " + value);
            }

            //Storing the received data from Intent to Bundle
            Bundle NotificationDataBundle = getIntent().getExtras();

            //Extracting the Bundle into variables
            Longitude = Double.parseDouble(NotificationDataBundle.getString("lng"));
            Latitude = Double.parseDouble(NotificationDataBundle.getString("lat"));
            NotificationMessageBody = NotificationDataBundle.getString("messageBody");

            //Assigning the extracted Longitude and Latitude variable to latlng object variable
            FirebaseMessageCoordinate = new LatLng(Latitude, Longitude);

            //Change the marker on the map and set marker title
            CurrentMarker.setPosition(FirebaseMessageCoordinate);
            CurrentMarker.setTitle(NotificationMessageBody);

            //Force the marker to show info window
            CurrentMarker.showInfoWindow();

            //Change the camera focus to the new latitude longitude location
            mMap.animateCamera(CameraUpdateFactory.newLatLngZoom(FirebaseMessageCoordinate, 15.2f));

        }
    }
}