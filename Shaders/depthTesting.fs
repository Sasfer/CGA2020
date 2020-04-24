#version 330 core
out vec4 FragColor;

float near = 0.1; 
float far = 100.0; 

// La siguente funci�n hace referencia a las distancias que hay entre el observador 
// y los planos lejano y cercano, de tal forma que se podr� realizar la transformaci�n 
// de las coordenadas respecto a esta distancia
float LinearizeDepth(float depth) 
{
	// Back to NDC 
    float z = depth * 2.0 - 1.0; 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}

void main()
{         
	// CASO de manejar distancias, de tal forma que podremos observar las variaciones 
	// de profundidad con la escala de grises, de tal forma que mientras m�s cercano 
	// este el valor cercano a cero ser� m�s cercano al observador, mientras que si 
	// es m�s cercano a uno estar� m�s lejano del observador.

	// gl_FragCoord.z el atributo Z de esta funci�n obtiene distancia
	// Divide by far to get depth in range [0,1] for visualization purposes
    float depth = LinearizeDepth(gl_FragCoord.z) / far; 
	FragColor = vec4(vec3(depth), 1.0);

	// CASO de manejar profuncidad, de tal forma que podremos observar el buffer de
	// profundidad en crudo, considerando que se tiene una transformaci�n de coordenadas
	// de 3D a 2D, realizada por la proyecci�n en perspectiva, con base al �rea de
	// dibujo del viewport.

	// En este caso observaremos la escena totalmente en blanco, sin embargo,
	// al acercanos lo suficiente a un modelo, se visualizar� un ligero color
	// gris, lo que nos permitir� notar cierta profundidad, no obstante, se
	// pierde lo que hay alrededor
	
	// gl_FragCoord.z el atributo Z de esta funci�n obtiene profundidad
    //FragColor = vec4(vec3(gl_FragCoord.z), 1.0);
}

