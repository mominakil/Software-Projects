/// @description 
if visible
{
draw_sprite(sprite_index, image_index, x, y);

var knob_amount = amount_current / amount_max;
var knob_position_x = x + (sprite_width * knob_amount);

draw_sprite(Sprite_Knob, is_being_dragged, knob_position_x, y);
}