// Copyright(C) David W. Jeske, 2014, All Rights Reserved.

#version 120
#extension GL_EXT_gpu_shader4 : enable

// edge distance from geometry shader..
noperspective varying vec3 f_dist;

uniform sampler2D diffTex;
uniform sampler2D specTex;
uniform sampler2D ambiTex;
uniform sampler2D bumpTex;

uniform int diffTexEnabled;
uniform int specTexEnabled;
uniform int ambiTexEnabled;
uniform int bumpTexEnabled;

uniform int showWireframes;
uniform float animateSecondsOffset;

// eye-space/cameraspace coordinates
varying vec3 f_VV;
varying vec3 f_vertexNormal;
varying vec3 f_lightPosition;
varying vec3 f_eyeVec;
varying vec3 f_vertexPosition_objectspace;

// tangent space vectors for bump mapping
varying vec3 surfaceLightVector;   
varying vec3 surfaceViewVector;
varying vec3 surfaceNormalVector;

// shadowmap related
uniform int shadowMapEnabled;
const int MAX_NUM_SHADOWMAPS = 4;
uniform int numShadowMaps;
uniform sampler2DShadow shadowMapTextures[MAX_NUM_SHADOWMAPS];
varying vec4 f_shadowMapCoords[MAX_NUM_SHADOWMAPS];


// http://www.clockworkcoders.com/oglsl/tutorial5.htm
// http://www.ozone3d.net/tutorials/bump_mapping_p4.php

//float rand(vec2 co){
//    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
//}

float rand(vec4 seed4) {
    float dot_product = dot(seed4, vec4(12.9898,78.233,45.164,94.673));
    return fract(sin(dot_product) * 43758.5453);
}

vec2 poissonDisk[16] = vec2[] (
    vec2(-0.04770581f, 0.1478396f),
    vec2(0.3363894f, 0.4504989f),
    vec2(-0.2229154f, 0.66614f),
    vec2(0.2214093f, -0.218469f),
    vec2(0.6464681f, 0.0007115538f),
    vec2(-0.4084882f, -0.2793796f),
    vec2(-0.6255119f, 0.1134195f),
    vec2(0.5613756f, -0.4556728f),
    vec2(0.7622108f, 0.4088926f),
    vec2(-0.01734736f, -0.7944852f),
    vec2(-0.8065997f, -0.4794133f),
    vec2(0.4379586f, 0.8250926f),
    vec2(0.3348362f, -0.9189008f),
    vec2(-0.3919142f, -0.9179187f),
    vec2(0.02141847f, 0.9828521f),
    vec2(-0.6499051f, 0.5785806f)    
 );


vec4 linearTest(vec4 outputColor) {
       float PI = 3.14159265358979323846264;
       vec4 effectColor = vec4(0.9);
	   float speedDivisor = 4.0f;
	   float pulseSpeedDivisor = 0.8f;
	   float pulseWidthDivisor = 3.0f;

	   float intensity = sin(mod (((f_vertexPosition_objectspace.z / pulseWidthDivisor) + (animateSecondsOffset / pulseSpeedDivisor)), PI));

       float proximity = mod(f_vertexPosition_objectspace.z + (animateSecondsOffset / speedDivisor), 1);
       if (proximity < 0.08) {
         outputColor = mix(effectColor,outputColor,clamp(proximity * 7.0,intensity,1));
       }
	return outputColor;
}

vec4 spiralTest(vec4 outputColor) {
  float time = animateSecondsOffset;
  vec2 resolution = vec2(2,2);
  vec2 aspect = vec2(2,2);
  vec4 effectColor = vec4(0.2);
  vec2 currentLocation = f_vertexPosition_objectspace.xy;
   
  vec2 position =  currentLocation / resolution.xy * aspect.xy;
  float angle = 0.0 ;
  float radius = length(position) ;
  if (position.x != 0.0 && position.y != 0.0){
    angle = degrees(atan(position.y,position.x)) ;
  }
  float amod = mod(angle+30.0*time-120.0*log(radius), 30.0) ;
  if (amod<15.0){
    outputColor += effectColor * clamp(log(radius), 0.0, 1.0) * clamp (amod / 30.0, 0, 1);
  } 
  return outputColor;
}

vec4 gridTest(vec4 outputColor) {
       float PI = 3.14159265358979323846264;
       vec4 effectColor = vec4(0.9);
       vec3 vp = f_vertexPosition_objectspace;
	   float speedDivisor = 2.0f;
	   float pulseSpeedDivisor = 1.0f;

	   float intensity = sin(mod ((animateSecondsOffset / pulseSpeedDivisor), PI));

       float a_prox = mod(vp.x + vp.y + (animateSecondsOffset / speedDivisor), 0.7);
	   float b_prox = mod(vp.z + vp.y + (animateSecondsOffset / speedDivisor), 0.7);

       if (a_prox < 0.07) {
         outputColor = mix(effectColor,outputColor,clamp(a_prox * 7.0,intensity,1));
       }
       
       if (b_prox < 0.07) {
         outputColor = mix(effectColor,outputColor,clamp(b_prox * 7.0,intensity,1));
       }
	   

	return outputColor;
}


void main()
{
	vec4 outputColor = vec4(0.0);

	// lighting strength
	vec4 ambientStrength = gl_FrontMaterial.ambient;
	vec4 diffuseStrength = gl_FrontMaterial.diffuse;
	vec4 specularStrength = gl_FrontMaterial.specular;
	// specularStrength = vec4(0.7,0.4,0.4,0.0);  // test red

	vec3 lightPosition = surfaceLightVector;

	// load texels...
	vec4 ambientColor = (diffTexEnabled == 1) ? texture2D (diffTex, gl_TexCoord[0].st) : vec4(0);
	vec4 diffuseColor = (diffTexEnabled == 1) ? texture2D (diffTex, gl_TexCoord[0].st) : vec4(0.5);

	vec4 glowColor    = (ambiTexEnabled == 1) ? texture2D (ambiTex, gl_TexCoord[0].st) : vec4(0);
	vec4 specTex      = (specTexEnabled == 1) ? texture2D (specTex, gl_TexCoord[0].st) : vec4(0);

	const float DEPTH_OFFSET = 0.01;
    const int numPoissionSamples = 4;
    const float sampleShadeFactor = 0.6 / numPoissionSamples;
    float shadeFactor = 1.0;
    vec3 seed3 = f_vertexPosition_objectspace.xyz * 1000.0;
	for (int i = 0; i < numShadowMaps; ++i) {
        vec4 coord = f_shadowMapCoords[i];
        float coordZ = (coord.z - DEPTH_OFFSET) / coord.w;
        for (int p = 0; p < numPoissionSamples; ++p) {
            int index = int(16.0*rand(vec4(seed3, p)))%16;
            vec2 offset = poissonDisk[index] / 700.0;
            vec4 shadowMapTexel = shadow2D(shadowMapTextures[i],
                                           vec3(coord.xy + offset, coordZ));
            shadeFactor -= (1.0 - shadowMapTexel.r) * sampleShadeFactor;
        }
    }

    if (true) {
	   // eye space shading
	   outputColor = ambientColor * ambientStrength;
	   outputColor += glowColor * gl_FrontMaterial.emission;

	   float diffuseIllumination = clamp(dot(f_vertexNormal, f_lightPosition), 0, 1);
	   // boost the diffuse color by the glowmap .. poor mans bloom
	   float glowFactor = length(gl_FrontMaterial.emission.xyz) * 0.2;
	   outputColor += shadeFactor * diffuseColor * diffuseStrength * max(diffuseIllumination, glowFactor);

	   // compute specular lighting
	   if (dot(f_vertexNormal, f_lightPosition) > 0.0) {   // if light is front of the surface
	  
	      vec3 R = reflect(-normalize(f_lightPosition), normalize(f_vertexNormal));
	      float shininess = pow (max (dot(R, normalize(f_eyeVec)), 0.0), gl_FrontMaterial.shininess);

	      // outputColor += specularStrength * shininess;
	      outputColor += shadeFactor * specTex * specularStrength * shininess;      
       } 


	} else {  // tangent space shading (with bump) 
       // lookup normal from normal map, move from [0,1] to  [-1, 1] range, normalize
       vec3 bump_normal = normalize( texture2D (bumpTex, gl_TexCoord[0].st).rgb * 2.0 - 1.0);
	   float distSqr = dot(surfaceLightVector,surfaceLightVector);
	   vec3 lVec = surfaceLightVector * inversesqrt(distSqr);

       // ambient ...
	   outputColor = ambientColor * ambientStrength;
	   outputColor += glowColor * gl_FrontMaterial.emission;
	          
       // diffuse...       
       float diffuseIllumination = clamp(dot(bump_normal,surfaceLightVector), 0,1);
       float glowFactor = length(gl_FrontMaterial.emission.xyz) * 0.2;
       outputColor += shadeFactor * diffuseColor * max(diffuseIllumination, glowFactor);

	   if (dot(bump_normal, surfaceLightVector) > 0.0) {   // if light is front of the surface

          // specular...
          vec3 R = reflect(-lVec,bump_normal);
          float shininess = pow (clamp (dot(R, normalize(surfaceViewVector)), 0,1), gl_FrontMaterial.shininess);
          outputColor += specTex * shadeFactor * specularStrength * shininess;      
       }

    }

    // ---- object space shader effect tests ----
    // outputColor = linearTest(outputColor);
    // outputColor = spiralTest(outputColor);
    // outputColor = gridTest(outputColor);

	// single-pass wireframe calculation
	// .. compute distance from fragment to closest edge
	if (showWireframes == 1) { 
		float edgeWidth = 1.5; // in screenspace pixels
		float nearD = min(min(f_dist[0],f_dist[1]),f_dist[2]);
        float curIntensity = max(max(outputColor.r,outputColor.g),outputColor.b);
		float edgeIntensity = exp2((-1 / edgeWidth)*nearD*nearD * 2);		
        vec4 edgeColor = vec4(vec3( (curIntensity < 0.4) ? 0.6 : 0.3 ),1.0);
		// vec4 edgeColor = vec4( clamp( (1.7 - length(outputColor.rgb) ),0.3,0.7) );			
        outputColor = mix(edgeColor,outputColor,1.0-edgeIntensity);
    }

	// finally, output the fragment color
    gl_FragColor = outputColor;
    //gl_FragColor = vec4(vec3(shadeFactor), 1);
}			

	
