extends Node2D

var coins: int = 0
var total_coins: int = 30


@onready var coin_label = $CanvasLayer/Label
#@onready var victory_screen = preload("res://scenes/VictoryScreen.tscn")

func increment_coin_count():
	coins += 1
	coin_label.text = "Coins: %d" % coins + "/30"
	
	if coins == total_coins:
		show_victory_screen()
	

func show_victory_screen():
	get_tree().change_scene_to_file("res://scenes/VictoryScreen.tscn")
