/// @description 
global.music_volume = 5;
global.sound_volume = 20;
visible = false;
cursor = cr_none;

amount_max = 100;
amount_current = 0;
is_being_dragged = false;

switch(settings) {
	case "music":
		amount_current = global.music_volume;
	break;
	case "sound":
		amount_current = global.sound_volume;
	break;
}

//audio_group_load(audioground_sfx);