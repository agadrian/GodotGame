extends Area2D


func _on_body_entered(body: Node2D) -> void:
	get_node("/root/Game").increment_coin_count()
	queue_free()
