global.car_speed = 0;
frame_count = 0;
frame_count_tree = 0;
distance = 0;

x = 947;
y = 800;
a = 15;

var i = 0;
repeat(5)
{
	new_x_r = 947;
	new_y_r = 285 + 50*i*i; //0 50 200 450 800
	line = instance_create_layer(new_x_r,new_y_r,"Blocks",obj_block);
	with(line)
	{	
		image_xscale = 0.00246*y - 0.5;
		image_yscale = 0.001*y - 0.111;
	}
	i++;
}

