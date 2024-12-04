extends Node2D

var coins: int = 0


@onready var coin_label = $CanvasLayer/Label

func increment_coin_count():
	coins += 1
	coin_label.text = "Monedas: %d" % coins + "/20"
