<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Emitters="ProjectMercury.Emitters" xmlns:Modifiers="ProjectMercury.Modifiers">
  <Asset Type="Emitters:Emitter">
    <Budget>10000</Budget>
    <Term>1</Term>
    <ReleaseQuantity>5</ReleaseQuantity>
    <ReleaseSpeed>
      <Anchor>20</Anchor>
      <Variation>1</Variation>
    </ReleaseSpeed>
    <ReleaseColour>0 1 0</ReleaseColour>
    <ReleaseOpacity>
      <Anchor>0.3</Anchor>
      <Variation>0</Variation>
    </ReleaseOpacity>
    <ReleaseScale>
      <Anchor>32</Anchor>
      <Variation>0.5</Variation>
    </ReleaseScale>
    <ReleaseRotation>
      <Anchor>0</Anchor>
      <Variation>0</Variation>
    </ReleaseRotation>
    <ParticleTextureAssetName>Effects\Textures\burst</ParticleTextureAssetName>
    <Modifiers>
      <Item Type="Modifiers:ColorModifier">
        <InitialColour>0 1 1</InitialColour>
        <UltimateColour>0 0.7529412 0</UltimateColour>
      </Item>
      <Item Type="Modifiers:OpacityModifier">
        <Initial>0.2</Initial>
        <Ultimate>0.05</Ultimate>
      </Item>
    </Modifiers>
  </Asset>
</XnaContent>