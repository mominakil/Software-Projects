		//if y>(1080 + 350)
		//{
		//	image_xscale = 0;
		//	image_yscale = 0;
		//	if x >945
		//	{
		//		x = 1040;
		//	}
		//	else
		//	{
		//		x = 870;
		//	}
		//}

	
		//if y<1000
		//{
		//	image_xscale = 0.001*y - 0.25;
		//	image_yscale = 0.001*y - 0.25;
		//}
if y<255 or y >748
{
	instance_destroy();
}

image_xscale = 0.001*y - 0.2;
image_yscale = 0.001*y - 0.2;
path_speed = global.car_speed * (sqrt(abs(y - 254)/50));
