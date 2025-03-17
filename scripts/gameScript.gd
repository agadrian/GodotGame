
extends Node2D

var coins: int = 0
var total_coins: int = 10
var player_id: int = 1
var player_name: String 

var http_request_put
var http_request_get
var http_request_getAll
var http_request_post


@onready var coin_label = $CanvasLayer/Label
@onready var previous_score_label = $CanvasLayer/Label2

func _ready():
	
	# Obtener todos los jugadores
	http_request_getAll = $HTTPRequestGetAll
	http_request_getAll.request_completed.connect(_on_getAll_request_completed)
	
	# Crear jugador
	http_request_post = $HTTPRequestPost
	http_request_post.request_completed.connect(_on_post_request_completed)
	
	# Editar puntuacion jugador
	http_request_put = $HTTPRequestPut
	http_request_put.request_completed.connect(_on_put_request_completed)
	
	# Obtener jugador
	http_request_get = $HTTPRequestGet
	http_request_get.request_completed.connect(_on_get_request_completed)
	
	check_or_create_player()
	
	#fetch_previous_score()

	

func increment_coin_count():
	coins += 1
	coin_label.text = "Coins: %d" % coins + "/30"
	
	if coins == total_coins:
		send_score_to_api()
		
		# Esperar un poco antes de mostrar pantalla de victoria para que de tiempo de procesar la solicitud de la API
		var timer = Timer.new()
		timer.wait_time = 0.6  
		timer.one_shot = true
		add_child(timer)
		timer.timeout.connect(show_victory_screen)
		timer.start()
		
		#show_victory_screen()
	

func show_victory_screen():
	call_deferred("deferred_show_victory_screen")

func deferred_show_victory_screen():
	get_tree().change_scene_to_file("res://scenes/VictoryScreen.tscn")


################################################################################
# Comprobar si hay jugadores creados en la API. Si hay, usarlo, si no, crearlo #
################################################################################

func check_or_create_player():
	print(" *-*-*- Comprobando si hay jugadores existentes...")
	var url = "https://web-api-psp.onrender.com/api/jugadores"
	var headers = ["Content-Type: application/json"]

	var error = http_request_getAll.request(url, headers, HTTPClient.METHOD_GET)
	if error != OK:
		print(" #GETALL - Error al enviar la solicitud GET"+ "\n\n")



func _on_getAll_request_completed(_result, response_code, _headers, body):
	print(" #GETALL - Respuesta GET recibida. Código: " + str(response_code))
	
	if response_code == 200:
		var json = JSON.new()
		var parse_result = json.parse(body.get_string_from_utf8())

		if parse_result == OK:
			var data = json.get_data()
			if data.size() > 0:  # Si hay...
				player_id = data[0]["id"]  # Usar el primero
				player_name = data[0]["nombre"]
				print(" #GETALL - Usando jugador existente con ID: " + str(player_id) + ", nombre: " + str(player_name)+ "\n\n")
				fetch_previous_score()
			else: # Si no hay, llamar a create_player()
				print(" #GETALL - No hay jugadores. Creando uno nuevo..."+ "\n\n")
				create_player()
		else:
			print(" #GETALL - No se pudo analizar la respuesta JSON"+ "\n\n")
	else:
		print(" #GETALL - Error al obtener jugadores: " + str(response_code)+ "\n\n")



func create_player():
	player_name = "Jugador 1"
	var url = "https://web-api-psp.onrender.com/api/jugadores"
	var headers = ["Content-Type: application/json"]
	var data = { "nombre": player_name, "maxScore": 0 }
	var json_data = JSON.stringify(data)

	print(" #POST - Enviando solicitud POST para crear jugador: " + json_data+ "\n\n")
	
	var error = http_request_post.request(url, headers, HTTPClient.METHOD_POST, json_data)
	if error != OK:
		print(" #POST - Error al enviar la solicitud POST"+ "\n\n")



func _on_post_request_completed(_result, response_code, _headers, body):
	print(" #POST - Código respuesta:" + str(response_code))
	
	if response_code == 201:
		var json = JSON.new()
		var parse_result = json.parse(body.get_string_from_utf8())
		
		if parse_result == OK:
			var data = json.get_data()
			if "id" in data:
				player_id = data["id"]
				print(" #POST - Jugador creado con ID: " + str(player_id)+ "\n\n")
				fetch_previous_score()
			else:
				print(" #POST - No se encontró 'id' en la respuesta"+ "\n\n")
		else:
			print(" #POST - No se pudo analizar la respuesta JSON"+ "\n\n")
	else:
		print(" #POST - Error al crear jugador: " + str(response_code))
		print(" #POST - Respuesta del servidor: " + body.get_string_from_utf8()+ "\n\n")



# Actualizar score API

func send_score_to_api():
	var url = "https://web-api-psp.onrender.com/api/jugadores/" + str(player_id)
	print(" #PUT - Enviando solicitud a: " + url) 
	
	var headers = ["Content-Type: application/json"]
	var data = { "id": player_id, "nombre": player_name, "maxScore": coins }
	var json_data = JSON.stringify(data)
	
	print(" #PUT - Enviando datos: " + json_data+ "\n\n")  
	
	var error = http_request_put.request(url, headers, HTTPClient.METHOD_PUT, json_data)

	
	if error != OK:
		print(" #PUT - Error al enviar la solicitud HTTP"+ "\n\n")



func _on_put_request_completed(_result, response_code, _headers, body):
	print(" #PUT - Respuesta recibida. Código: " + str(response_code))
	
	if response_code == 204:
		print(" #PUT - Score actualizado correctamente en la API"+ "\n\n")
	else:
		print(" #PUT - Error al actualizar el score: " + str(response_code))
		print(" #PUT - Respuesta del servidor: " + body.get_string_from_utf8())



# Obtener score API

func fetch_previous_score():
	print(" *-*-*- Llamando a fetch_previous_score()")  
	var url = "https://web-api-psp.onrender.com/api/jugadores/" + str(player_id)
	print(" #GET - Obteniendo previous_score desde: " + url)
	
	var headers = ["Content-Type: application/json"]
	
	var error = http_request_get.request(url, headers, HTTPClient.METHOD_GET)
	
	if error != OK:
		print(" #GET - Error al enviar la solicitud GET para obtener previous_score"+ "\n\n")



func _on_get_request_completed(_result, response_code, _headers, body):
	print(" #GET - Respuesta de GET recibida. Código: " + str(response_code))
	
	if response_code == 200:
		var json = JSON.new()
		var parse_result = json.parse(body.get_string_from_utf8())
		
		if parse_result == OK:
			var data = json.get_data()
			if "maxScore" in data:
				var previous_score = data["maxScore"]
				print(" #GET - Previous Score obtenido: " + str(previous_score) + "\n\n")
				previous_score_label.text = "Previous Score: " + str(previous_score)
				
				if "nombre" in data:
					player_name = data["nombre"]  
			else:
				print(" #GET - No se encontró 'maxScore' en la respuesta"+ "\n\n")
		else:
			print(" #GET - No se pudo analizar la respuesta JSON"+ "\n\n")
	else:
		print(" #GET - Error al obtener previous_score: " + str(response_code)+ "\n\n")
