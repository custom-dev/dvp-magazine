<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:mag="http://tempuri.org/Magazine.xsd">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/">    
    <package xmlns="http://www.idpf.org/2007/opf" version="2.0" unique-identifier="bookid">
      <metadata xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:opf="http://www.idpf.org/2007/opf">
        <dc:title>Magazine n°<xsl:value-of select="/mag:Magazine/mag:Numero/text()" /></dc:title>
        <dc:creator opf:role="aut">Alain Perrin</dc:creator>
        <dc:creator opf:role="rev">f-leb</dc:creator>
        <dc:subject/>
        <dc:description>Retour sur le développement et l'élaboration du jeu indépendant et amateur de lilington : Soul of Mask. De l'idée à la publication sur Steam</dc:description>
        <dc:publisher>Developpez.com</dc:publisher>
        <dc:date opf:event="publication">2018-06-05</dc:date>
        <dc:type>Text</dc:type>
        <dc:format>application/xhtml+xml</dc:format>
        <dc:identifier id="bookid">https://jeux.developpez.com/making-of/soul-of-mask/</dc:identifier>
        <dc:language>fr</dc:language>
        <dc:rights>
          Les sources présentées sur cette page sont libres de droits et vous pouvez les utiliser à votre convenance. Par contre, la page de présentation constitue une œuvre intellectuelle protégée par les droits d'auteur. Copyright ® 2018 Alain Perrin. Aucune reproduction, même partielle, ne peut être faite de ce site et de l'ensemble de son contenu : textes, documents, images, etc. sans l'autorisation expresse de l'auteur. Sinon vous encourez selon la loi jusqu'à trois ans de prison et jusqu'à 300 000 € de dommages et intérêts.
        </dc:rights>
      </metadata>
      <manifest>
        <item id="couverture" href="couverture.xml" media-type="application/xhtml+xml"/>
      </manifest>
      <spine>
        <itemref idref="couverture"/>        
      </spine>      
    </package>
  </xsl:template>

</xsl:stylesheet>