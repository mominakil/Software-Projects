/// @desc Establish main car movement
randomize();
key_left = keyboard_check_pressed(vk_left);
key_right = keyboard_check_pressed(vk_right);
key_up = keyboard_check_pressed(vk_up);
key_down = keyboard_check_pressed(vk_down);
key_jump = keyboard_check_pressed(vk_space);

//control main car
if key_left and y == 800 and (x == 947 or x == 1407)
{
	path_start(move_left,70,path_action_stop,false);
	audio_play_sound(drifting,100,false);
	audio_sound_gain(drifting, global.sound_volume / 100, 0);
	with(obj_shadow){
	path_start(move_left,70,path_action_stop,false);}
}

if key_right and y == 800 and (x == 947 or x == 487)
{
	audio_play_sound(drifting,100,false);
	audio_sound_gain(drifting, global.sound_volume / 100, 0);
	path_start(move_right,70,path_action_stop,false);
	with(obj_shadow){
	path_start(move_right,70,path_action_stop,false);}

}

if key_up and global.car_speed<=14
{
	
	global.car_speed++;
}

if key_down and global.car_speed >0
{
	global.car_speed--;
}

if key_jump and y ==800 and (x == 947 or x == 487 or x == 1407)
{
	audio_play_sound(Jumping,100,false);
	audio_sound_gain(Jumping, global.sound_volume / 100, 0);
	path_start(jump_path,0,path_action_stop,false);
	path_speed = a - a*(sqrt((1600-2*y)/a)); 

}

//generate enemy cars
frame_count++;
frame_count_tree++;
distance += frame_count*(global.car_speed-1.4)
if distance > 9000
{
	frame_count = 0;
	distance = 0;
	if global.level>=4{
		if random(10)<8 event_user(0);
		if random(10)<8 event_user(1);
		if random(10)<8 event_user(2);}
	else{
		if random(10)<global.level+4 event_user(0);
		if random(10)<global.level+4 event_user(1);
		if random(10)<global.level+4 event_user(2);
	}
}
if frame_count_tree>(250/global.car_speed) and global.car_speed >0
{
	frame_count_tree = 0;
	if random(10)>6 event_user(3);
	if random(100)>60 event_user(4);
}


//if global.car_speed
//{
//	var _car_speed = global.car_speed;
//	with(obj_movement) {
		
//		if x > 960 
//		{	

//			x += _car_speed * (0.001*y - 0.05);
//			y = 0.88 * x - 661;
//		} 
//		else 
//		{
//			x -= _car_speed * (0.001*y - 0.05);
//			y = - 0.9 * x + 1021; 		
//		}
		
//	}
//}

//collision part

