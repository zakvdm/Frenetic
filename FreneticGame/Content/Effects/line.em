<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Emitters="ProjectMercury.Emitters" xmlns:Modifiers="ProjectMercury.Modifiers">
  <Asset Type="Emitters:LineEmitter">
    <Budget>1000</Budget>
    <Term>0.5</Term>
    <ReleaseQuantity>500</ReleaseQuantity>
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
      <Anchor>4</Anchor>
      <Variation>1.5</Variation>
    </ReleaseScale>
    <ReleaseRotation>
      <Anchor>0</Anchor>
      <Variation>0</Variation>
    </ReleaseRotation>
    <ParticleTextureAssetName>Effects\point</ParticleTextureAssetName>
    <Modifiers>
      <Item Type="Modifiers:ColorModifier">
        <InitialColour>0.8627451 0.0784313753 0.235294119</InitialColour>
        <UltimateColour>0 0.7529412 0</UltimateColour>
      </Item>
      <Item Type="Modifiers:OpacityModifier">
        <Initial>0.2</Initial>
        <Ultimate>0.05</Ultimate>
      </Item>
    </Modifiers>
    <Length>300</Length>
    <Angle>0</Angle>
    <Rectilinear>false</Rectilinear>
    <EmitBothWays>false</EmitBothWays>
  </Asset>
</XnaContent>