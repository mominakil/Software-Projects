///@desc Create right trees
var random_tree = instance_create_layer(1022,255,"Trees",obj_tree);
with(random_tree)
{
	image_xscale = 0.001*y - 0.2;
	image_yscale = 0.001*y - 0.2;
	path_start(tree_right,0,path_action_continue,true);
}
	