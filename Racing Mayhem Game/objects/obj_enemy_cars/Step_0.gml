	image_xscale = 0.00217*y - 0.54;
	image_yscale = 0.00217*y - 0.54;
	if y<266 or y >849
	{
		instance_destroy();
	}
	//if x <952 and x>945 //601.140583 473.026426
	//{
	//	path_speed = 0.787 * (global.car_speed * (sqrt(abs(y - 265)/50)) - 1.4*(sqrt(abs(y - 265)/50)));
	//}
	//else
	//{
	//	path_speed = global.car_speed * (sqrt(abs(y - 265)/50)) - 1.4*(sqrt(abs(y - 265)/50));
	//}
	if x <952 and x>945 //601.140583 473.026426
	{
		path_speed = 0.787 *(global.car_speed * (tan((y-250)*pi/1200)) - 1.4*(tan((y-250)*pi/1200)));
	}
	else
	{
		path_speed = global.car_speed * (tan((y-250)*pi/1200)) - 1.4*(tan((y-250)*pi/1200));
	}
	
	if y>710
	{
		if place_meeting(x,y,obj_main_car){
			//instance_deactivate_all( false );
			//instance_create_layer(947,522,"Instances",obj_replay_button);
			
			if global.car_speed != 1
			{
				instance_create_layer(x,y,"Crash_effect",obj_crash_effect);
				obj_crash_effect.alarm[0] = 25;
				audio_play_sound(crash_sound_effect,100,false);
				audio_sound_gain(crash_sound_effect, global.sound_volume / 100, 0);
				global.car_speed = 1;
			}		
		}
	}
