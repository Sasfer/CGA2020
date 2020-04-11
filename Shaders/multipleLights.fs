#version 330 core
// Hace referencia a la version de GLSG

struct Light {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct DirectionalLight{
    vec3 direction;
    Light light;
};

struct PointLight{
    vec3 position;
    
	Light light;
	
	float constant;
    float linear;
    float quadratic;
};

struct SpotLight{
	vec3 position;
	vec3 direction;

	Light light;

	// Transici�n entre la luz y la oscuridad
	float cutOff;
	float outerCutOff;

	float constant;
    float linear;
    float quadratic;

};

const int MAX_POINT_LIGHTS = 20;
const int MAX_SPOT_LIGHTS = 20;

out vec4 color;

// El contenido de esta varaibles provienen del VF
in vec3 fragPos;  
in vec3 our_normal;
// Coordenadas de texturas
in vec2 our_uv;

// Variables globales que pueden cambiar en tiempo de ejecuci�n
uniform int pointLightCount;
uniform int spotLightCount;

// Para cambiar sus propiedades desde el programa principal
uniform DirectionalLight directionalLight;
uniform PointLight pointLights[MAX_POINT_LIGHTS];
uniform SpotLight spotLights[MAX_SPOT_LIGHTS];

// Posici�n de observador, hace referencia a la camara
uniform vec3 viewPos;  
// Objeto que obtiene la textura
uniform sampler2D texture1;

vec3 calculateDirectionalLight(Light light, vec3 direction){
	// Ambient
    vec3 ambient  = light.ambient * vec3(texture(texture1, our_uv));
  	
    // Diffuse 
    vec3 normal = normalize(our_normal);
    vec3 lightDir = normalize(-direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse  = light.diffuse * (diff * vec3(texture(texture1, our_uv)));
    
    // Specular
	// Se calcula un vector de reflexi�n
    float specularStrength = 0.5f;
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, normal);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);
    vec3 specular = light.specular * (spec * vec3(texture(texture1, our_uv)));  
        
    return (ambient + diffuse + specular);
}

vec3 calculatePointLights(){
	vec3 result = vec3(0, 0, 0);
	for(int i = 0; i < pointLightCount; i++){
		vec3 lightDir = normalize(fragPos - pointLights[i].position );
		float distance = length(pointLights[i].position - fragPos);
		// Se calcula la atenuaci�n
		float attenuation = 1.0f / (pointLights[i].constant + pointLights[i].linear * distance + pointLights[i].quadratic * distance * distance);
		result +=  attenuation * calculateDirectionalLight(pointLights[i].light, lightDir);
	}
	return result;
}

vec3 calculateSpotLights(){
	vec3 result = vec3(0, 0, 0);
	for(int i = 0; i < spotLightCount; i++){
		vec3 lightDir = normalize(spotLights[i].position - fragPos);
		float theta = dot(lightDir, normalize(-spotLights[i].direction));   
		float epsilon   = spotLights[i].cutOff - spotLights[i].outerCutOff;
		float intensity = clamp((theta - spotLights[i].outerCutOff) / epsilon, 0.0, 1.0);   
		float distance = length(spotLights[i].position - fragPos);
		// Para que se pueda emplear como una PointLight
		float attenuation = 1.0f / (spotLights[i].constant + spotLights[i].linear * distance + spotLights[i].quadratic * distance * distance);
		result +=  intensity * attenuation * calculateDirectionalLight(spotLights[i].light, spotLights[i].direction);
	}
	return result;
}

void main()
{	 
	vec4 colorText = texture(texture1, our_uv);
	// IMPORTANTE
	// Si se quieren decartar transparencias quitar if,
	// considerar manejar este aspecto directamente en el modelo
	//if(colorText.a < 0.1)
		//discard;
    color = vec4(calculateDirectionalLight(directionalLight.light, directionalLight.direction)
	+ calculatePointLights() + calculateSpotLights(), colorText.a);
}