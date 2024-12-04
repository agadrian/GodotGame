extends Area2D


func _on_body_entered(body: Node2D) -> void:
	print("fdgfdg")
	get_node("/root/Game/Player").Die()
	
