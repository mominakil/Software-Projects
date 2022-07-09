draw_self();
draw_set_font(fnt_score);
draw_text(x-70, y - 57, "Scoreboard");
draw_text(x - 180, y - 27, string("Score: ") + string(round(score_display)));


draw_text(x - 180, y + 20, string("Distance:") + string(round(distance))+ "m");
draw_text_transformed(x+40, y-10, string(global.level),1.5,1.5,0);
//draw_text_color(x,y,string(timer),c_red,c_red,c_red,c_red,1);
draw_text_color(x+125,y-45,"Time",c_black,c_black,c_black,c_black,1);


if timer<=5 and timer>0{
	if !sound_on{
	audio_play_sound(sound_timer,50,false);
	audio_sound_gain(sound_timer, global.sound_volume / 100, 0);
	sound_on = true;
	}
	draw_text_transformed_color(x+125,y-15,string(timer),2,2,0,c_red,c_red,c_red,c_red,1);
}
else if timer==0{
	audio_stop_sound(bgm);
	audio_stop_sound(sound_timer);
	audio_play_sound(timesup,10,false);
	audio_sound_gain(timesup, global.sound_volume / 100, 0);
	instance_deactivate_all( false );
	//draw_text_transformed_color(900,500,string("Time's up!"),1.5,1.5,0,c_red,c_red,c_red,c_red,1);
	//draw_sprite(Sprite_gameover,image_index ,945,200);
	instance_create_layer(945,300,"Score_board",obj_gameover);
	instance_create_layer(945,612,"Score_board",obj_replay_btn);
	instance_create_layer(945,790,"Score_board",obj_back_btn);
}

else{
	
	audio_stop_sound(sound_timer);
	sound_on = false;
	draw_text_transformed_color(x+125,y-15,string(timer),2,2,0,c_black,c_black,c_black,c_black,1);
}
