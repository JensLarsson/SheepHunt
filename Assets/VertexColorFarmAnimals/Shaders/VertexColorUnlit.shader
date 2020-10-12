Shader "VertexColorFarmAnimals/VertexColorUnlit" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
}

Category{
	Tags { "Queue" = "Geometry" }
	Lighting Off
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex

	}

	SubShader {
		Pass {

			SetTexture [_MainTex] {
				combine primary 
				 constantColor[_Color]
			  combine constant lerp(texture) previous
			}
		// Multiply in texture
		SetTexture[_MainTex] {
			combine previous * primary
		}
		}
	}
}
}