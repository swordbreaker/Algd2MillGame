﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;
using Action = MillGame.Models.Core.Actions.Action;

namespace MillGame.Models
{
    public class GameNode : Node<Core.Actions.Action>, IGameNode
    {
        protected int m_score;

        public GameNode(Core.Actions.Action action) : base(action)
        {
        }

        public GameNode(Core.Actions.Action action, int score) : base(action)
        {
            m_score = score;
        }

        /**
        * Create new nodes recursively.
        * Approximate O(24*(height-curHeight))
        * @param curHeight current subtree height
        * @param height Subtree height
        * @param color Color of next actions
        * @param root Subtree root
        * @param rootState Game state at root
        * @return Number of created nodes
        */
        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState)
        {
            int numberOfCreatedNodes = 0;
            if(curHeight != height && !rootState.Finished())
            {
                if (color == IController.WHITE)
                {
                    // White is minimizer
                    // Todo -> Wher to select minimizer maximizer?
                    if (rootState.PlacingPhase(color) || rootState.JumpingPhase(color))
                    {
                        foreach(byte position in State.TRANSPOSED)
                        {
                            if(rootState.IsValidPlace(position, color))
                            {
                                GameNode childNode = root.Add(new Placing(color, position));
                                State newState = rootState.clone();
                                childNode.Data().Update(newState);
                                numberOfCreatedNodes++;
                                numberOfCreatedNodes += Create(curHeight++, height, State.OppositeColor(color), childNode, newState);
                            }
                        }
                    }
                    else if (rootState.MovingPhase(color))
                    {
                        for(int i = 0; i < State.TRANSPOSED.Length; i++)
                        {
                            foreach(byte to in State.MOVES[i])
                            {
                                if(rootState.IsValidMove(State.TRANSPOSED[i], to, color))
                                {
                                    GameNode childNode = root.Add(new Moving(color, State.TRANSPOSED[i], to));
                                    State newState = rootState.clone();
                                    childNode.Data().Update(newState);
                                    numberOfCreatedNodes++;
                                    numberOfCreatedNodes += Create(curHeight++, height, State.OppositeColor(color), childNode, newState);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Black is maximizer
                    // Todo -> Wher to select minimizer maximizer?
                    if (rootState.PlacingPhase(color) || rootState.JumpingPhase(color))
                    {
                        foreach (byte position in State.TRANSPOSED)
                        {
                            if (rootState.IsValidPlace(position, color))
                            {
                                GameNode childNode = root.Add(new Placing(color, position));
                                State newState = rootState.clone();
                                childNode.Data().Update(newState);
                                numberOfCreatedNodes++;
                                numberOfCreatedNodes += Create(curHeight++, height, State.OppositeColor(color), childNode, newState);
                            }
                        }
                    }
                    else if (rootState.MovingPhase(color))
                    {
                        for (int i = 0; i < State.TRANSPOSED.Length; i++)
                        {
                            foreach (byte to in State.MOVES[i])
                            {
                                if (rootState.IsValidMove(State.TRANSPOSED[i], to, color))
                                {
                                    GameNode childNode = root.Add(new Moving(color, State.TRANSPOSED[i], to));
                                    State newState = rootState.clone();
                                    childNode.Data().Update(newState);
                                    numberOfCreatedNodes++;
                                    numberOfCreatedNodes += Create(curHeight++, height, State.OppositeColor(color), childNode, newState);
                                }
                            }
                        }
                    }
                }
            }
            return numberOfCreatedNodes;
        }

        /**
	     * Create new node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Action a, int score)
        {
            // TODO Add minimizer or maximizer?
            GameNode node = new GameNode(a, score);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        public GameNode Add(Action a)
        {
            // TODO Add minimizer or maximizer?
            GameNode node = new GameNode(a);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        /*
         * Removes now unused subtrees
         * O(n)
         */
        public GameNode RemoveUnusedChilds(Action a)
        {
            GameNode node = null;
            foreach(GameNode child in m_children)
            {
                if (child.Data() != a)
                {
                    m_children.Remove(child);
                } else
                {
                    node = child;
                }
            }
            return node;
        }

        /**
	     * Compute game state at this node
	     * @param s Game state at given node v
	     * @param v Game node v must be parent of this
	     * @return Game state at this node
	     */
        public State ComputeState(State s, GameNode v)
        {
            State computedState = s.clone();
            v.Data().Update(computedState);
            return computedState;
        }

        public int GetWinnerScore()
        {
            // Yanick: Why is this method here? I don't see the use of it. The winner store is stored in the state which is only available in the GameTree!
            // Can I delete it? It's not in the class diagram but in the interface.. WTF
            // Or maybe just compute the winner score (none, black or white)? But then I have to some how get a initial state an ancestor node of this to calculate this value..
            // Take from minimizer / maximizer?
            throw new NotImplementedException();
        }

        public int Score()
        {
            return m_score;
        }

        /**
         * If this score > other score  then return > 0
         * If this score < other score  then return < 0
         * If this score == other score  then return 0
         */
        public override int CompareTo(Node<Action> other)
        {
            GameNode gameNodeOther = (GameNode) other;
            return m_score - gameNodeOther.m_score;
        }
    }
}
