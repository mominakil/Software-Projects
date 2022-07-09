distance = distance - global.car_speed * 0.05;
score_display = score_display + global.car_speed * 0.05;
if distance<=0
{	
	audio_play_sound(success_bell,10,false);
	audio_sound_gain(success_bell, global.sound_volume / 100, 0);
	global.level++;
	if global.level>=4 distance = 780;
	else distance = 580 + 50 * global.level;
	timer += 22;
}