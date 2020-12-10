
//// TODO: TBD: started from the OrTools OrToolsRoutingCaseStudyTests<> class originally...
//// TODO: TBD: however, if we follow this through and have a legit Svg based charting component...
//// TODO: TBD: then that really deserves its own dedicated repository...
//// TODO: TBD: what we end up with here is a really rough draft attempt at starting some charting layouts...
//// TODO: TBD: including basic comprehension of viewport dimensions, two dimensional X and Y axes, major and minor hash marks, and so forth...
//// TODO: TBD: we think this should be refactored into a class oriented scaffold, with proper first class wrappers around the Svg details...
//// TODO: TBD: then, also, with series interpretation, given a series of T objects, how those translate to XY coordinates, values, etc...
//// TODO: TBD: and with what sort of series comprehension... i.e. in this case scatter plot, but others as well, potentially..., spline, etc
//// TODO: TBD: and on from there...

        // TODO: TBD: render the series in 1-1 scale...
        // TODO: TBD: then we can worry about the parent element scale, view port, etc...
        // TODO: TBD: this should really be its own thing...
        // TODO: TBD: if we run with SVG in mind, and  the more we get acquainted...
        // TODO: TBD: could put a charting scaffold around the SVG aspects...
        // TODO: TBD: an adapter layer between the charting subscriber and the SVG aspects...
        protected virtual void RenderSvg(TScope scope
            , float height = 1000f, float width = 1000f
            , float strokeWidth = 1f
            , float x = default, float y = default
            , int? majorHash = null, int? minorHash = null)
        {
            const float zed = default;

            Svg.SvgGroup RenderGridGroup()
            {
                //// TODO: TBD: we can add some labels to the axes...
                //group.Children.Add(new Svg.SvgText { Text = "" });

                // TODO: TBD: if we stick with Svg...
                // TODO: TBD: could do extension methods like .From(...), .To(...), etc.

                Svg.SvgGroup RenderGridLineGroup()
                {
                    var gridLines = new Svg.SvgGroup();

                    // TODO: TBD: starting with the axis lines themselves...
                    IEnumerable<Svg.SvgElement> RenderAxisLines()
                    {
                        // X axis
                        yield return new Svg.SvgLine { StartX = zed, StartY = height, EndX = width, EndY = height };

                        // Y axis
                        yield return new Svg.SvgLine { StartX = zed, StartY = height, EndX = zed, EndY = zed };
                    }

                    IEnumerable<Svg.SvgElement> RenderAllHashLines()
                    {
                        IEnumerable<Svg.SvgElement> RenderHashLines(int deltaX, int deltaY, int deltaHash)
                        {
                            yield break;
                        }

                        // TODO: TBD: add the major and minor lines...
                        yield break;
                    }

                    RenderAxisLines().ToList().ForEach(gridLines.Children.Add);
                    RenderAllHashLines().ToList().ForEach(gridLines.Children.Add);

                    return gridLines;
                }

                IEnumerable<Svg.SvgElement> RenderAxisHashMarks(int deltaX, int deltaY, int deltaHash)
                {
                    (float x, float y) CalculateNext((float x, float y) coord)
                    {
                        if (deltaX == default && deltaY != default)
                        {
                            return (coord.x, coord.y - deltaY);
                        }
                        else if (deltaX != default && deltaY == default)
                        {
                            return (coord.x + deltaX, coord.y);
                        }

                        // We allow only one configuration or the other, never both.
                        throw new InvalidOperationException();
                    }

                    for (var coord = (x: zed, y: height); (deltaX != default && coord.x < width)
                        || (deltaY != default && coord.y > zed); coord = CalculateNext(coord))
                    {
                        if (deltaX == default)
                        {
                            yield return new Svg.SvgLine { StartX = coord.x - deltaHash, StartY = coord.y, EndX = coord.x + deltaHash, EndY = coord.y };
                        }
                        else if (deltaY == default)
                        {
                            yield return new Svg.SvgLine { StartX = coord.x, StartY = coord.y + deltaHash, EndX = coord.x, EndY = coord.y - deltaHash };
                        }
                    }
                }

                Svg.SvgGroup RenderHashMarkGroup()
                {
                    var major = majorHash ?? default;
                    var minor = minorHash ?? default;

                    // TODO: TBD: we'll have to see if this works...
                    // TODO: TBD: and how it looks and feels, etc...
                    const int majorWidth = 16;
                    const int minorWidth = majorWidth / 2;

                    var allHashMarks = new Svg.SvgGroup();

                    if (majorHash != null)
                    {
                        RenderAxisHashMarks(major, default, majorWidth / 2).ToList().ForEach(allHashMarks.Children.Add);
                        RenderAxisHashMarks(default, major, majorWidth / 2).ToList().ForEach(allHashMarks.Children.Add);
                    }

                    if (minorHash != null)
                    {
                        RenderAxisHashMarks(minor, default, minorWidth / 2).ToList().ForEach(allHashMarks.Children.Add);
                        RenderAxisHashMarks(default, minor, minorWidth / 2).ToList().ForEach(allHashMarks.Children.Add);
                    }

                    return allHashMarks;
                }

                // TODO: TBD: to which we could provide a set of transforms...
                var group = new Svg.SvgGroup { Transforms = { } };

                Range<Svg.SvgElement>(RenderGridLineGroup(), RenderHashMarkGroup())
                    .ToList().ForEach(group.Children.Add);

                return group;
            }

            Svg.SvgGroup RenderLocationGroup()
            {
                IEnumerable<Svg.SvgElement> RenderAllLocations()
                {
                    // TODO: TBD: could be the actual indices of the matrix... i.e. TSP...
                    // TODO: TBD: or informed elsewise, i.e. circuit board...

                    yield break;
                }

                var locationGroup = new Svg.SvgGroup();

                RenderAllLocations().ToList().ForEach(locationGroup.Children.Add);

                return locationGroup;
            }

            Svg.SvgGroup RenderRouteGroup()
            {
                IEnumerable<Svg.SvgElement> RenderAllRoutes()
                {
                    // TODO: TBD: we think must be partially informed by at least the matrix... i.e. TSP ...
                    // TODO: TBD: if not by a separate set of coordinates... i.e. CB...
                    yield break;
                }

                var routes = new Svg.SvgGroup();

                RenderAllRoutes().ToList().ForEach(routes.Children.Add);

                return routes;
            }

            //// image? or doc? and then, how is it we render it? as a png? bitmap? or otherwise...
            //var doc = new Svg.SvgDocument();

            // TODO: TBD: allow for XY label margins...
            var image = new Svg.SvgImage { X = x, Y = y, Width = width, Height = height, StrokeWidth = strokeWidth };

            // TODO: TBD: assuming there is some sort of z ordering going on here...
            Range<Svg.SvgElement>(RenderGridGroup(), RenderLocationGroup(), RenderRouteGroup())
                .ToList().ForEach(image.Children.Add);
        }