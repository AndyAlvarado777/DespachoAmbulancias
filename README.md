# DespachoAmbulancias

# Proyecto PED: Sistema de Despacho de Ambulancias

Bienvenidos al repositorio del Grupo C. Este proyecto es para la Fase 2 de la materia Programación con Estructuras de Datos de la UDB Virtual. 

Básicamente, estamos construyendo un sistema que modela las calles como un grafo para mandar la ambulancia disponible más cercana a una emergencia, y luego llevar al paciente al hospital más rápido.

## El Equipo
# Andy Alfredo Alvarado Diaz - AD262401
# Eduardo Ernesto Henríquez Cañas - HH132104
# Jefferson André Gonzalez Guillén - GG252398
# Marvin Salvador Gonzalez Zetino - GZ221985
# Juan Carlos Monge Ventura - MV253544

** ¿Qué estamos implementando? **
Para que el motor de búsqueda sea eficiente, combinamos varias estructuras:
1 - Grafos dirigidos y ponderados: Para representar las intersecciones y el tiempo que toma cruzarlas.
2 - Dijkstra con terminación temprana: Para encontrar la ruta óptima sin tener que calcular todo el mapa por gusto.
3 -  Colas de prioridad: Usamos una para que Dijkstra saque el menor tiempo, y otra para ordenar las emergencias por nivel de urgencia (rojo, amarillo, verde).
4 - Pilas: Para darle vuelta a la ruta al final y que quede en el orden correcto de viaje.

** Reglas internas del grupo **
Muchachos, recordemos cómo nos repartimos el trabajo para salir a tiempo:
1 - Nadie toca master directo: Cada quien debe trabajar en su propia rama (ej. marvin-grafo, andy-entidades).
2 - Commits seguidos: Asegúrense de hacer commits pequeños y continuos en sus piezas para que quede la evidencia del avance.
3 - Integración: El viernes unimos todo en master y hacemos las pruebas del Main para grabar el video el sábado.