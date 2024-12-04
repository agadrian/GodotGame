extends Node2D

var coins: int = 0
var total_coins: int = 3


@onready var coin_label = $CanvasLayer/Label
#@onready var victory_screen = preload("res://scenes/VictoryScreen.tscn")

func increment_coin_count():
	coins += 1
	coin_label.text = "Monedas: %d" % coins + "/20"
	
	if coins == total_coins:
		print("123123123123123123")
		show_victory_screen()

func show_victory_screen():
	var timer = Timer.new()
	timer.wait_time = 1.0  # Espera 1 segundo antes de cambiar de escena
	timer.one_shot = true
	timer.connect("timeout", Callable(self, "_on_victory_screen_timeout"))
	add_child(timer)  # Agregamos el temporizador como hijo del nodo actual
	timer.start()
	
func _on_victory_screen_timeout():
	# Cambiar de escena
	get_tree().change_scene_to_file("res://scenes/VictoryScreen.tscn")
