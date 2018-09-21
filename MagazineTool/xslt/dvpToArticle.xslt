<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">
    <Article>
      <MainTitle><xsl:value-of select="/document/entete/titre/article/text()"/></MainTitle>
      <Description><xsl:value-of select="/document/entete/meta/description/text()"/></Description>
      <AlternativeTitles>
        <AlternativeTitle type="html"><xsl:value-of select="/document/entete/titre/page/text()"/></AlternativeTitle>
      </AlternativeTitles>
      <Revisions>
        <Revision>
          <xsl:attribute name="date"><xsl:value-of select="/document/entete/date/text()"/></xsl:attribute>
        </Revision>
        <Revision>
          <xsl:attribute name="date">
            <xsl:value-of select="/document/entete/miseajour/text()"/>
          </xsl:attribute>
        </Revision>
      </Revisions>
      <Synopsis>
        <xsl:apply-templates select="/document/synopsis" mode="synopsis" /> 
      </Synopsis>
    </Article>
  </xsl:template>
  
  <xsl:template match="paragraph" mode="synopsis">
    <p>
      <xsl:apply-templates mode="inline" />
    </p>
  </xsl:template>

  <xsl:template match="font[@color]" mode="inline">
    <color>
      <xsl:attribute name="value"><xsl:value-of select="@color" /></xsl:attribute>
      <xsl:apply-templates mode="inline"   />
    </color>
  </xsl:template>

  <xsl:template match="lien-forum" mode="inline">
    <link>
      <xsl:attribute name="url">https://www.developpez.net/forums/showthread.php?t=<xsl:value-of select="@id"/></xsl:attribute>
      <text>Commentez</text>
    </link>
  </xsl:template>

  <xsl:template match="b" mode="inline">
    <strong>
      <xsl:apply-templates mode="inline"/>
    </strong>  
  </xsl:template>

  <xsl:template match="i" mode="inline">
    <emphasis>
      <xsl:apply-templates mode="inline"/>
    </emphasis>
  </xsl:template>
  
  <xsl:template match="text()" mode="inline">
    <xsl:choose>
      <xsl:when test="normalize-space(.)">
        <text>
          <xsl:value-of select="."/>
        </text>
      </xsl:when>
    </xsl:choose>    
  </xsl:template>
</xsl:stylesheet>
