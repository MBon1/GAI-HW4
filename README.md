# GAI-HW4
Game AI Homework IV Pathfinding

================================================================

  +------------+
  | HOMEWORK 4 |
  +------------+

================================================================

GROUP MEMBERS

  Matthew Bonnecaze   (bonnem3)
  Justin Hung        (hungj2)
  Phillip Stapleton  (staplp2)

================================================================

LINKS

WebGL Build

  https://bonnem.itch.io/pathfinding-bonnecaze-hung-stapleton

GitHub Repository

  https://github.com/MBon1/GAI-HW4

================================================================

INSTRUCTIONS

  From the map selection screen, select a map, either waypoint mode or grid mode, and a node size. The default node size is 2, indicating a node two tiles wide and two tiles tall. The node size can be changed by entering an integer value into the text field under “NODE SIZE” followed by hitting enter. The new node size resets to 2 upon returning to the map selection screen.

  Once the map has loaded, use the scroll wheel to zoom in and out and the arrow keys to pan in the cardinal directions. Press the escape key to reset both the camera zoom and position to the origin. Press Q to return to the map selection screen.

  To run the A* algorithm, right click a node to select a start point, then right click another node to select an endpoint. The algorithm will immediately run after the endpoint is selected. (A new start point and endpoint can be selected upon completion.)

  Please note that nodes with green tiles (trees) or gray tiles (out of bounds blockages) are considered obstacles. Choosing a green or gray node as a start point or endpoint will immediately result in A* failure, and no calculations will be performed.

  Further settings can be adjusted from the submenu in the top left. The H weight can be set by entering a new H weight value into the corresponding text field and then pressing “Enter”. By default, the H weight is 1.0. By default, the A* algorithm uses Euclidean distance as its heuristic. The heuristic can be set by selecting a heuristic from the Heuristic dropdown. 

  Hover the cursor over a node to see its position and its G, H, and F values within this box. The Time Scale value can also be found in this menu. Pressing “=” will increase the time scale by 1 while pressing “-” will decrease the time scale by 1. Selecting the checked toggle on the top right of the editor window can collapse the editor window. To reopen the window, select the unchecked toggle. You can check if the path finding algorithm is running or if a path was found by checking the text next to “A*” in the editor window. If the algorithm is running, the text will read “RUNNING”. If the algorithm found a path, the text will read “PATH FOUND”. If the algorithm could not find a path, the text will read “NO PATH FOUND”. If the start and end points are not set, this text will be a set of ellipses. 

  NOTE: This demonstration is best experienced when using full-screen mode.

================================================================

LEGEND

  +-------------+------------------+------------------------------+
  | COLOR       | TYPE             | NOTES                        |
  +-------------+------------------+------------------------------+
  | DARK YELLOW | start point      | also successful path color   |
  | MAGENTA     | end point        |                              |
  | RED         | open list node   |                              |
  | BLUE        | closed list node |                              |
  | WHITE       | normal tile      |                              |
  | GREEN       | tree             | obstacle / unselectable tile |
  | GRAY        | out of bounds    | obstacle / unselectable tile |
  | PURPLE      | waypoint node    | waypoint mode only           |
  +-------------+------------------+------------------------------+

ADDITIONAL NOTES

  When A* is completed, a line will be drawn from the start to the end. The magenta end of the line is the start point, and the cyan end of the line is the endpoint.

================================================================

MAP REPRESENTATIONS

Tile-Based Representation

  Our tile-based map representation simply splits the map into nodes, from the given size (default is 2x2 tiles), starting from the top left corner. Each node will contain multiple tiles many of which may be of different types. Nodes with no non-traversable tiles will be reachable by our algorithm while any node with one or more non-traversable tiles will be considered non-traversable. This means that partially blocked tiles are unreachable by our A* implementation. 

  We have chosen to do this so that our implementation does not break from the tile-based mindset of discretizing a map structure into chunks of tiles. (Professor Si verified our implementation as being acceptable during office hours.) Our current program has no requirement to move through partially blocked tiles and so to break our implementation to handle this is unnecessary. Our implementation could also easily model a game with larger characters who would have hard times squeezing through small partially blocked spaces. 

  If we were to consider smaller agents, we would have to break nodes into smaller subsets of traversable nodes, which it would be able to travel through. If a map were to have more directions to walk in than just the 4 cardinal or 4 diagonal directions, we would have to change our implementation to represent them correctly and attempt to break the tiles into nodes in different packaging while also changing how neighbors of tiles are connected.

Waypoint-Based Representation

  Waypoints are determined by scanning the map for "convex corners." Convex corners are defined as any tile with an obstructed immediate northeast, northwest, southwest, or southeast neighbor, where the two tiles adjacent to that neighbor and to the original node are unobstructed. (As an example, a node is considered to be at a convex corner if its northeast neighbor is obstructed, but both its north neighbor and east neighbor are unobstructed.)

  Once all waypoints are determined, we determine all pairs of waypoints (waypoints A and B) and cast a ray from A to B. If the ray does not hit any green or gray obstacle tiles, then edge AB is considered valid and A and B are assigned to be each other’s neighbors in the greater A* graph. Otherwise, waypoints A and B are left disconnected.

  It is important to note that one drawback of Unity's ray casting system is that if a wall is perfectly tangent to a ray from one waypoint to another, the ray will not actually intersect with the ray. This behavior, while it may look slightly strange, virtually never causes any real issues with pathfinding.

  Furthermore, compared to the tile-based representation, our implementation of the waypoint-based representation is slightly more computationally expensive since each of the waypoints' neighbors need to be determined at start time and between setting the destination node and performing the A* algorithm.

================================================================

A* IMPLEMENTATION

  Our A* algorithm is modeled similarly to the one from the video shown in class. Our algorithm does not revisit nodes in the closed list to check if their f values are lower since once a node has been put into the closed list it has been at its lowest possible F value. This is due to there being no differing costs to travel between nodes. Each node travel costs the same and, for the tile-based representation of the map, we only allow for travel in the cardinal directions making A* even faster as it only has to search 4 neighbors for the lowest F values instead of all 8 surrounding a node.

  We chose to only allow travel in the cardinal directions since valuing travel in diagonals would require adding differing costs to node travel since diagonal travel is sqrt(2) distance instead of 1. This implementation still gives the best pathfinding for travel in the cardinal directions (in which many games are restricted similarly).

  Our implementation of the waypoint-based representation calculates all neighboring waypoints on map load and recalculates them before performing the A* algorithm to consider the reachable nodes from the start and end points. This is a costly operation, and if time permitted, we would have liked to look further into speeding this operation up; however, once the neighbors are determined, determining a path between the start and end nodes is much faster than when using a tile-based world representation as there are fewer nodes to check. 

  One possible alternative solution to the above problem would have been to base our map on a hexagon-based grid (which would not have worked very well with the provided map files), but Professor Si suggested we simply only allow movement in the four cardinal directions for the tile-based world representation instead.

  If a map were to have differing terrain or circumstances that would allow for travel from some nodes to other have varying terrain costs, our implementation of A* would have to change. We would have to allow for revisiting nodes in the closed list since certain paths may have a lower cost to it that would be searched later. When using a waypoint based representation of the world, our current implementation of A* or representation of the world does not factor in the tiles between waypoints other than if there are no obstructions between two waypoints.  If the walkable tiles did have different terrain-based weights, when determining neighbors, we would need to determine the terrains of the tiles between two waypoints and factor that in when determining the neighbors, giving each neighboring node a weight to read it as a direct line between two waypoints might not be the faster approach anymore.  

================================================================

HEURISTICS

Euclidean Distance 
	
  Euclidean Distance will find the shortest distance between two points, the direct line between the two. For our purposes we used the Vector3.Distance() function which calculates this distance using the Pythagorean Theorem. This heuristic works great in all sorts of implementations.

  (The Euclidian distance from point A to point B in the figure below is 5.)
  
  sqrt((3 * 3) + (4 * 4))
  sqrt(9 + 16)
  sqrt(25)
  5

Manhattan Distance

  Manhattan Distance, unlike Euclidean, is the addition of the X and Y coordinates of two points to find the shortest distance between them only using cardinal directions. For this we simply took the absolute value of the difference of each coordinate to calculate their distances. This heuristic is great for use in a tile-based game where cardinal directions are only allowed/promoted.

  (The Manhattan distance from point A to point B in the figure below is 7.)

  3 + 4
  7

FIGURE

  +---+---+---+---+
  | o---o---o---B |
  +-|-+---+---+---+
  | o |   |   |   |
  +-|-+---+---+---+
  | o |   |   |   |
  +-|-+---+---+---+
  | o |   |   |   |
  +-|-+---+---+---+
  | A |   |   |   |
  +---+---+---+---+

================================================================

WEIGHTING HEURISTICS

  For our implementation we allow the user to choose the weight of the H component for calculating F during A*. The H component refers to the distance from the current node being looked at to the goal. This is unique for A* and differentiates it from other algorithms such as Dijkstra’s which simply uses the G component (distance from start). Weighting the H component less will make the algorithm act more like Dijkstra and search for nodes close to the start. Weighting H more will cause the algorithm to seek the goal strongly, potentially finding a path faster, but it may not be the shortest path.

================================================================

EXTRA CREDIT

Modifiable Node Size

  Our implementation allows for the user to modify the size of nodes that the map will be built using. The default size of a node is 2x2 tiles. The user may input their wanted size in the text input field in the map select screen, and when they select a map to go to, the tiles in the map will be packaged in nodes of the requested size. Note that nodes are squares (meaning that the node size value is the length and width of the node). The node size value can only be an integer value greater than or equal to 1. If you attempt to set an invalid node size, the input text field will not accept the input. However, if the input is a floating point number, it will floor the value and set it to 1 if the floored value is less than 1. The node size value will reset back to 2 upon returning to the map select screen. 

================================================================

RELEVANT FUNCTIONS

  AStar.AStarCoroutine();
  AStar.ManhattanDistance();
  AStarWindow.SetTargetProperty();
  Map.getNeighbors();
  Map.setNeighbors();
  PathFindingMouseController.Update();
  TileMapLoader.LoadTileMap();
  TileMapLoader.SetHWeight();
  TileSizeSetter.SetValue();

================================================================
