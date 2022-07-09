///@desc Create enemy cars

var enemy_car = instance_create_layer(978,266,"enemy_cars",obj_enemy_car_1);
with(enemy_car){
	image_xscale = 0.00217*y - 0.54;
	image_yscale = 0.00217*y - 0.54;
	path_start(enemy_car_path_right,150,path_action_continue,true);
	path_speed = 10*(sqrt((y - 265)/50));
}