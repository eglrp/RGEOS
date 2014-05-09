using System;
using System.IO;
using RGeos.Geometries;

namespace RGeos.Converters.WellKnownText
{
    /// <summary>
    /// Outputs the textual representation of a <see cref="RGeos.Geometries.Geometry"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>The Well-Known Text (WKT) representation of Geometry is designed to exchange geometry data in ASCII form.</para>
    /// Examples of WKT representations of geometry objects are:
    /// <list type="table">
    /// <listheader><term>Geometry </term><description>WKT Representation</description></listheader>
    /// <item><term>A Point</term>
    /// <description>POINT(15 20)<br/> Note that point coordinates are specified with no separating comma.</description></item>
    /// <item><term>A LineString with four points:</term>
    /// <description>LINESTRING(0 0, 10 10, 20 25, 50 60)</description></item>
    /// <item><term>A Polygon with one exterior ring and one interior ring:</term>
    /// <description>POLYGON((0 0,10 0,10 10,0 10,0 0),(5 5,7 5,7 7,5 7, 5 5))</description></item>
    /// <item><term>A MultiPoint with three Point values:</term>
    /// <description>MULTIPOINT(0 0, 20 20, 60 60)</description></item>
    /// <item><term>A MultiLineString with two LineString values:</term>
    /// <description>MULTILINESTRING((10 10, 20 20), (15 15, 30 15))</description></item>
    /// <item><term>A MultiPolygon with two Polygon values:</term>
    /// <description>MULTIPOLYGON(((0 0,10 0,10 10,0 10,0 0)),((5 5,7 5,7 7,5 7, 5 5)))</description></item>
    /// <item><term>A GeometryCollection consisting of two Point values and one LineString:</term>
    /// <description>GEOMETRYCOLLECTION(POINT(10 10), POINT(30 30), LINESTRING(15 15, 20 20))</description></item>
    /// </list>
    /// </remarks>
    public class GeometryToWKT
    {
        #region Methods

        /// <summary>
        /// Converts a Geometry to its Well-known Text representation.
        /// </summary>
        /// <param name="geometry">A Geometry to write.</param>
        /// <returns>A &lt;Geometry Tagged Text&gt; string (see the OpenGIS Simple
        ///  Features Specification)</returns>
        public static string Write(IGeometry geometry)
        {
            StringWriter sw = new StringWriter();
            Write(geometry, sw);
            return sw.ToString();
        }

        /// <summary>
        /// Converts a Geometry to its Well-known Text representation.
        /// </summary>
        /// <param name="geometry">A geometry to process.</param>
        /// <param name="writer">Stream to write out the geometry's text representation.</param>
        /// <remarks>
        /// Geometry is written to the output stream as &lt;Gemoetry Tagged Text&gt; string (see the OpenGIS
        /// Simple Features Specification).
        /// </remarks>
        public static void Write(IGeometry geometry, StringWriter writer)
        {
            AppendGeometryTaggedText(geometry, writer);
        }

        /// <summary>
        /// Converts a Geometry to &lt;Geometry Tagged Text &gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="geometry">The Geometry to process.</param>
        /// <param name="writer">The output stream to Append to.</param>
        private static void AppendGeometryTaggedText(IGeometry geometry, StringWriter writer)
        {
            if (geometry == null)
                throw new NullReferenceException("Cannot write Well-Known Text: geometry was null");
            ;
            if (geometry is Point)
            {
                Point point = geometry as Point;
                AppendPointTaggedText(point, writer);
            }
            else if (geometry is LineString)
                AppendLineStringTaggedText(geometry as LineString, writer);
            else if (geometry is Polygon)
                AppendPolygonTaggedText(geometry as Polygon, writer);
            else if (geometry is MultiPoint)
                AppendMultiPointTaggedText(geometry as MultiPoint, writer);
            else if (geometry is MultiLineString)
                AppendMultiLineStringTaggedText(geometry as MultiLineString, writer);
            else if (geometry is MultiPolygon)
                AppendMultiPolygonTaggedText(geometry as MultiPolygon, writer);
            else if (geometry is GeometryCollection)
                AppendGeometryCollectionTaggedText(geometry as GeometryCollection, writer);
            else
                throw new NotSupportedException("Unsupported Geometry implementation:" + geometry.GetType().Name);
        }

        /// <summary>
        /// Converts a Coordinate to &lt;Point Tagged Text&gt; format,
        /// then Appends it to the writer.
        /// </summary>
        /// <param name="coordinate">the <code>Coordinate</code> to process</param>
        /// <param name="writer">the output writer to Append to</param>
        private static void AppendPointTaggedText(Point coordinate, StringWriter writer)
        {
            writer.Write("POINT ");
            AppendPointText(coordinate, writer);
        }

        /// <summary>
        /// Converts a LineString to LineString tagged text format, 
        /// </summary>
        /// <param name="lineString">The LineString to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendLineStringTaggedText(LineString lineString, StringWriter writer)
        {
            writer.Write("LINESTRING ");
            AppendLineStringText(lineString, writer);
        }

        /// <summary>
        ///  Converts a Polygon to &lt;Polygon Tagged Text&gt; format,
        ///  then Appends it to the writer.
        /// </summary>
        /// <param name="polygon">Th Polygon to process.</param>
        /// <param name="writer">The stream writer to Append to.</param>
        private static void AppendPolygonTaggedText(Polygon polygon, StringWriter writer)
        {
            writer.Write("POLYGON ");
            AppendPolygonText(polygon, writer);
        }

        /// <summary>
        /// Converts a MultiPoint to &lt;MultiPoint Tagged Text&gt;
        /// format, then Appends it to the writer.
        /// </summary>
        /// <param name="multipoint">The MultiPoint to process.</param>
        /// <param name="writer">The output writer to Append to.</param>
        private static void AppendMultiPointTaggedText(MultiPoint multipoint, StringWriter writer)
        {
            writer.Write("MULTIPOINT ");
            AppendMultiPointText(multipoint, writer);
        }

        /// <summary>
        /// Converts a MultiLineString to &lt;MultiLineString Tagged
        /// Text&gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="multiLineString">The MultiLineString to process</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendMultiLineStringTaggedText(MultiLineString multiLineString, StringWriter writer)
        {
            writer.Write("MULTILINESTRING ");
            AppendMultiLineStringText(multiLineString, writer);
        }

        /// <summary>
        /// Converts a MultiPolygon to &lt;MultiPolygon Tagged
        /// Text&gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="multiPolygon">The MultiPolygon to process</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendMultiPolygonTaggedText(MultiPolygon multiPolygon, StringWriter writer)
        {
            writer.Write("MULTIPOLYGON ");
            AppendMultiPolygonText(multiPolygon, writer);
        }

        /// <summary>
        /// Converts a GeometryCollection to &lt;GeometryCollection Tagged
        /// Text&gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="geometryCollection">The GeometryCollection to process</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendGeometryCollectionTaggedText(GeometryCollection geometryCollection,
                                                               StringWriter writer)
        {
            writer.Write("GEOMETRYCOLLECTION ");
            AppendGeometryCollectionText(geometryCollection, writer);
        }


        /// <summary>
        /// Converts a Coordinate to Point Text format then Appends it to the writer.
        /// </summary>
        /// <param name="coordinate">The Coordinate to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendPointText(Point coordinate, StringWriter writer)
        {
            if (coordinate == null || coordinate.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                AppendCoordinate(coordinate, writer);
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a Coordinate to &lt;Point&gt; format, then Appends
        /// it to the writer. 
        /// </summary>
        /// <param name="coordinate">The Coordinate to process.</param>
        /// <param name="writer">The output writer to Append to.</param>
        private static void AppendCoordinate(Point coordinate, StringWriter writer)
        {
            for (uint i = 0; i < coordinate.NumOrdinates; i++)
                writer.Write(WriteNumber(coordinate[i]) + (i < coordinate.NumOrdinates - 1 ? " " : ""));
        }

        /// <summary>
        /// Converts a double to a string, not in scientific notation.
        /// </summary>
        /// <param name="d">The double to convert.</param>
        /// <returns>The double as a string, not in scientific notation.</returns>
        private static string WriteNumber(double d)
        {
            return "";
            //return d.ToString(Map.NumberFormatEnUs);
        }

        /// <summary>
        /// Converts a LineString to &lt;LineString Text&gt; format, then
        /// Appends it to the writer.
        /// </summary>
        /// <param name="lineString">The LineString to process.</param>
        /// <param name="writer">The output stream to Append to.</param>
        private static void AppendLineStringText(LineString lineString, StringWriter writer)
        {
            if (lineString == null || lineString.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                for (int i = 0; i < lineString.NumPoints; i++)
                {
                    if (i > 0)
                        writer.Write(", ");
                    AppendCoordinate(lineString.Vertices[i], writer);
                }
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a Polygon to &lt;Polygon Text&gt; format, then
        /// Appends it to the writer.
        /// </summary>
        /// <param name="polygon">The Polygon to process.</param>
        /// <param name="writer"></param>
        private static void AppendPolygonText(Polygon polygon, StringWriter writer)
        {
            if (polygon == null || polygon.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                AppendLineStringText(polygon.ExteriorRing, writer);
                for (int i = 0; i < polygon.InteriorRings.Count; i++)
                {
                    writer.Write(", ");
                    AppendLineStringText(polygon.InteriorRings[i], writer);
                }
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a MultiPoint to &lt;MultiPoint Text&gt; format, then
        /// Appends it to the writer.
        /// </summary>
        /// <param name="multiPoint">The MultiPoint to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendMultiPointText(MultiPoint multiPoint, StringWriter writer)
        {
            if (multiPoint == null || multiPoint.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                for (int i = 0; i < multiPoint.Points.Count; i++)
                {
                    if (i > 0)
                        writer.Write(", ");
                    AppendCoordinate(multiPoint[i], writer);
                }
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a MultiLineString to &lt;MultiLineString Text&gt;
        /// format, then Appends it to the writer.
        /// </summary>
        /// <param name="multiLineString">The MultiLineString to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendMultiLineStringText(MultiLineString multiLineString, StringWriter writer)
        {
            if (multiLineString == null || multiLineString.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                for (int i = 0; i < multiLineString.LineStrings.Count; i++)
                {
                    if (i > 0)
                        writer.Write(", ");
                    AppendLineStringText(multiLineString[i], writer);
                }
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a MultiPolygon to &lt;MultiPolygon Text&gt; format, then Appends to it to the writer.
        /// </summary>
        /// <param name="multiPolygon">The MultiPolygon to process.</param>
        /// <param name="writer">The output stream to Append to.</param>
        private static void AppendMultiPolygonText(MultiPolygon multiPolygon, StringWriter writer)
        {
            if (multiPolygon == null || multiPolygon.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                for (int i = 0; i < multiPolygon.Polygons.Count; i++)
                {
                    if (i > 0)
                        writer.Write(", ");
                    AppendPolygonText(multiPolygon[i], writer);
                }
                writer.Write(")");
            }
        }

        /// <summary>
        /// Converts a GeometryCollection to &lt;GeometryCollection Text &gt; format, then Appends it to the writer.
        /// </summary>
        /// <param name="geometryCollection">The GeometryCollection to process.</param>
        /// <param name="writer">The output stream writer to Append to.</param>
        private static void AppendGeometryCollectionText(GeometryCollection geometryCollection, StringWriter writer)
        {
            if (geometryCollection == null || geometryCollection.IsEmpty())
                writer.Write("EMPTY");
            else
            {
                writer.Write("(");
                for (int i = 0; i < geometryCollection.Collection.Count; i++)
                {
                    if (i > 0)
                        writer.Write(", ");
                    AppendGeometryTaggedText(geometryCollection[i], writer);
                }
                writer.Write(")");
            }
        }

        #endregion
    }
}