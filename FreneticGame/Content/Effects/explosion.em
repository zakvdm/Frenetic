<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Emitters="ProjectMercury.Emitters" xmlns:Modifiers="ProjectMercury.Modifiers">
  <Asset Type="Emitters:CircleEmitter">
    <Budget>5000</Budget>
    <Term>0.5</Term>
    <ReleaseQuantity>180</ReleaseQuantity>
    <ReleaseSpeed>
      <Anchor>10</Anchor>
      <Variation>1</Variation>
    </ReleaseSpeed>
    <ReleaseColour>0.8627451 0.0784313753 0.235294119</ReleaseColour>
    <ReleaseOpacity>
      <Anchor>0.2</Anchor>
      <Variation>0</Variation>
    </ReleaseOpacity>
    <ReleaseScale>
      <Anchor>32</Anchor>
      <Variation>4</Variation>
    </ReleaseScale>
    <ReleaseRotation>
      <Anchor>0</Anchor>
      <Variation>0</Variation>
    </ReleaseRotation>
    <ParticleTextureAssetName>Effects\Textures\burst</ParticleTextureAssetName>
    <Modifiers>
      <Item Type="Modifiers:ColorModifier">
        <InitialColour>1 0.8 0</InitialColour>
        <UltimateColour>0.25 0.1 0</UltimateColour>
      </Item>
      <Item Type="Modifiers:OpacityModifier">
        <Initial>0.2</Initial>
        <Ultimate>0.05</Ultimate>
      </Item>
    </Modifiers>
    <Radius>50</Radius>
    <Ring>false</Ring>
    <Radiate>true</Radiate>
  </Asset>
</XnaContent>