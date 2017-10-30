using System;
using System.Collections;
using System.Collections.Generic;

namespace VacuumAgent
{
    public class DirectedWeightedGraph<T>
    {
        private const long EmptyEdgeSlot = 0;
        private const object EmptyNodeSlot = null;

        private long[,] _adjacencyMatrix;

        private readonly int _capacity;
        
        private ArrayList _nodes;

        public DirectedWeightedGraph(uint capacity = 100)
        {
            _capacity = (int) capacity;
            Clear();
        }
        
        /*
         * Properties.
         */

        public int NodesCount { get; private set; }

        public int EdgesCount { get; private set; }

        public IEnumerable<WeightedEdge<T>> Edges
        {
            get
            {
                foreach (var node in _nodes)
                {
                    foreach (var outgoingEdge in OutgoingEdges((T) node))
                    {
                        yield return outgoingEdge;
                    }
                }
            }
        }

        public IEnumerable<T> Nodes
        {
            get
            {
                foreach (var node in _nodes)
                {
                    if (node != null)
                        yield return (T) node;
                }
            }
        }
        
        /*
         * Utility functions.
         */

        /// <summary>
        /// Check if an edge exists based on indexes.
        /// </summary>
        /// <param name="source">Source node index.</param>
        /// <param name="destination">Destination node index.</param>
        /// <returns>True if the edge exists, false otherwise.</returns>
        private bool DoesEdgeExists(int source, int destination)
        {
            return _adjacencyMatrix[source, destination] != EmptyEdgeSlot;
        }

        /// <summary>
        /// Get the edge weight based on indexes.
        /// </summary>
        /// <param name="source">Source node index.</param>
        /// <param name="destination">Destination node index.</param>
        /// <returns></returns>
        private long GetEdgeWeightByIndex(int source, int destination)
        {
            return _adjacencyMatrix[source, destination];
        }
        
        /*
         * Graph-related methods.
         */

        /// <summary>
        /// Return the edges going to a node.
        /// </summary>
        /// <param name="node">Destination node.</param>
        /// <returns>The edges as an IEnumerable.</returns>
        public IEnumerable<WeightedEdge<T>> IncomingEdges(T node)
        {
            if (!HasNode(node))
                yield return null;

            var source = _nodes.IndexOf(node);

            for (var adjacent = 0; adjacent < _nodes.Count; adjacent++)
            {
                if (_nodes[adjacent] != null && DoesEdgeExists(adjacent, source))
                {
                    yield return new WeightedEdge<T>(
                        (T) _nodes[adjacent],
                        node,
                        GetEdgeWeightByIndex(source, adjacent)
                    );
                }
            }
        }

        /// <summary>
        /// Return the edges going from a node.
        /// </summary>
        /// <param name="node">Source node.</param>
        /// <returns>The edges as an IEnumerable.</returns>
        public IEnumerable<WeightedEdge<T>> OutgoingEdges(T node)
        {
            if (!HasNode(node))
                yield return null;

            var source = _nodes.IndexOf(node);

            for (var adjacent = 0; adjacent < _nodes.Count; adjacent++)
            {
                if (_nodes[adjacent] != null && DoesEdgeExists(adjacent, source))
                {
                    yield return new WeightedEdge<T>(
                        node,
                        (T) _nodes[adjacent],
                        GetEdgeWeightByIndex(source, adjacent)
                    );
                }
            }
        }

        /// <summary>
        /// Add an edge to the graph.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="destination">Destination node.</param>
        /// <param name="weight">Weight of the edge.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool AddEdge(T source, T destination, long weight)
        {
            // Check for the empty edge default value
            if (weight == EmptyEdgeSlot)
                return false;

            var sourceIndex = _nodes.IndexOf(source);
            var destIndex = _nodes.IndexOf(destination);

            // Check for existance of nodes and non-existance of the edge
            if (sourceIndex == -1 || destIndex == -1)
                return false;
            
            if (DoesEdgeExists(sourceIndex, destIndex))
                return false;

            _adjacencyMatrix[sourceIndex, destIndex] = weight;
            EdgesCount++;

            return true;
        }

        /// <summary>
        /// Update the weight of an edge.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="destination">Destination node.</param>
        /// <param name="weight">New weight.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool UpdateEdgeWeight(T source, T destination, long weight)
        {
            // Check for the empty edge default value
            if (weight == EmptyEdgeSlot)
                return false;
            
            var sourceIndex = _nodes.IndexOf(source);
            var destIndex = _nodes.IndexOf(destination);
            
            // Check for existance of nodes and non-existance of the edge
            if (sourceIndex == -1 || destIndex == -1)
                return false;
            
            if (!DoesEdgeExists(sourceIndex, destIndex))
                return false;

            _adjacencyMatrix[sourceIndex, destIndex] = weight;

            return true;
        }

        /// <summary>
        /// Remove an edge.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="destination">Destination node.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveEdge(T source, T destination)
        {
            var sourceIndex = _nodes.IndexOf(source);
            var destIndex = _nodes.IndexOf(destination);
            
            // Check for existance of nodes and non-existance of the edge
            if (sourceIndex == -1 || destIndex == -1)
                return false;
            
            if (!DoesEdgeExists(sourceIndex, destIndex))
                return false;

            _adjacencyMatrix[sourceIndex, destIndex] = EmptyEdgeSlot;
            EdgesCount--;

            return true;
        }

        /// <summary>
        /// Return an edge.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="destination">Destination node.</param>
        /// <returns>The edge.</returns>
        public WeightedEdge<T> GetEdge(T source, T destination)
        {
            var sourceIndex = _nodes.IndexOf(source);
            var destIndex = _nodes.IndexOf(destination);
            
            // Check for existance of nodes and non-existance of the edge
            if (sourceIndex == -1 || destIndex == -1)
                return null;
            
            if (!DoesEdgeExists(sourceIndex, destIndex))
                return null;
            
            return new WeightedEdge<T>(source, destination, GetEdgeWeightByIndex(sourceIndex, destIndex));
        }

        /// <summary>
        /// Compute the weight of an edge.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="destination">Destination node.</param>
        /// <returns>The weight of the edge.</returns>
        public long GetEdgeWeight(T source, T destination)
        {
            return GetEdge(source, destination).Weight;
        }
        
        /// <summary>
        /// Check if an edge exists from source to destination.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="destination">Destination node.</param>
        /// <returns>True if an edge exists, false otherwise.</returns>
        public bool HasEdge(T source, T destination)
        {
            var sourceIndex = _nodes.IndexOf(source);
            var destIndex = _nodes.IndexOf(destination);

            return sourceIndex != -1 && destIndex != -1 && DoesEdgeExists(sourceIndex, destIndex);
        }

        /// <summary>
        /// Add multiple nodes to the graph.
        /// </summary>
        /// <param name="nodeCollection">A IList contaiing the nodes.</param>
        /// <exception cref="ArgumentNullException">Occur when the input collection is null.</exception>
        public void AddNodes(IList<T> nodeCollection)
        {
            if (nodeCollection == null)
                throw new ArgumentNullException();

            foreach (var node in nodeCollection)
                AddNode(node);
        }

        /// <summary>
        /// Add a node to the graph.
        /// </summary>
        /// <param name="node">Node to be added.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool AddNode(T node)
        {
            if (NodesCount >= _capacity)
                return false;

            if (HasNode(node))
                return false;

            _nodes.Add(node);
            NodesCount++;

            return true;
        }

        /// <summary>
        /// Remove a node from the graph.
        /// </summary>
        /// <param name="node">Node to be removed.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveNode(T node)
        {
            if (NodesCount == 0)
                return false;

            var index = _nodes.IndexOf(node);

            if (index == -1)
                return false;

            _nodes[index] = EmptyNodeSlot;
            NodesCount--;
            
            // Remove all edges connected to this node
            for (var i = 0; i < _capacity; i++)
            {
                // Source edge
                if (DoesEdgeExists(index, i))
                {
                    _adjacencyMatrix[index, i] = EmptyEdgeSlot;
                    EdgesCount--;
                }
                
                // Destination edge
                if (DoesEdgeExists(i, index))
                {
                    _adjacencyMatrix[i, index] = EmptyEdgeSlot;
                    EdgesCount--;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if a node exists.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>True if it exists, false otherwise.</returns>
        public bool HasNode(T node)
        {
            return _nodes.Contains(node);
        }

        /// <summary>
        /// Return the neighbors of a node.
        /// </summary>
        /// <param name="node">Input node.</param>
        /// <returns>A LinkedList containing the neighbors.</returns>
        public LinkedList<T> Neighbors(T node)
        {
            var neighbors = new LinkedList<T>();
            var source = _nodes.IndexOf(node);

            if (source != -1)
            {
                for (var adjacent = 0; adjacent < _nodes.Count; adjacent++)
                {
                    if (_nodes[adjacent] != null && DoesEdgeExists(source, adjacent))
                    {
                        neighbors.AddLast((T) _nodes[adjacent]);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Return the neighbors of a node as a dictionary of nodes-to-weights.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Dictionary<T, long> NeighborsMap(T node)
        {
            if (!HasNode(node))
                return null;
            
            var neighbors = new Dictionary<T, long>();
            var source = _nodes.IndexOf(node);

            if (source != -1)
            {
                for (var adjacent = 0; adjacent < _nodes.Count; adjacent++)
                {
                    if (_nodes[adjacent] != null && DoesEdgeExists(source, adjacent))
                    {
                        neighbors.Add((T) _nodes[adjacent], GetEdgeWeightByIndex(source, adjacent));
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Clear graph.
        /// </summary>
        public void Clear()
        {
            EdgesCount = 0;
            NodesCount = 0;
            _nodes = new ArrayList(_capacity);
            _adjacencyMatrix = new long[_capacity, _capacity];
            
            // Initialize adjacency matrix
            for (var i = 0; i < _capacity; i++)
            {
                for (var j = 0; j < _capacity; j++)
                {
                    _adjacencyMatrix[i, j] = EmptyEdgeSlot;
                }
            }
        }
    }
}