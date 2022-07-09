if y>(1080 + 150)
		{
			image_xscale = 0;
			image_yscale = 0;
			
			if x >947
			{
				x = 970;
				var mirror_line;
				mirror_line = instance_create_layer(924,240,main_room,obj_line);
				with(mirror_line){
					image_xscale = 0;
					image_yscale = 0;
					image_angle = -18;
					if y> (1080+150)
					{
						instance_destroy();
					}
				}
			}

		}
		
	
		if y<1000
		{
			image_xscale = 0.001*y - 0.25;
			image_yscale = 0.001*y - 0.25;
		}