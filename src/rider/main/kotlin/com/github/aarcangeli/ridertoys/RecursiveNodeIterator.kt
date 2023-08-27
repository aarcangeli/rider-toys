package com.github.aarcangeli.ridertoys

import com.intellij.lang.ASTNode
import com.intellij.psi.PsiComment
import com.intellij.psi.PsiWhiteSpace
import com.intellij.psi.impl.source.tree.LeafElement
import com.intellij.psi.tree.IElementType

class RecursiveNodeIterator(rootElement: ASTNode) {
    private val nodes: MutableList<LeafElement?>
    private var index = 0

    init {
        nodes = getAllNodes(rootElement)
        // Add EOF
        nodes.add(null)
    }

    fun advanceElement() {
        assert(!eof())
        index++
    }

    val previousElement: LeafElement?
        get() {
            assert(index > 0)
            return nodes[index - 1]
        }
    val currentElement: LeafElement?
        get() = if (index < nodes.size) nodes[index] else null

    fun currentElementType(): IElementType? {
        return if (index < nodes.size) nodes[index]!!.elementType else null
    }

    fun nextElementType(): IElementType? {
        return if (index + 1 < nodes.size) nodes[index + 1]!!.elementType else null
    }

    fun lookAhead(count: Int): IElementType? {
        assert(!eof())
        return if (index + count < nodes.size) nodes[index + count]!!.elementType else null
    }

    fun eof(): Boolean {
        return index == nodes.size - 1
    }

    companion object {
        private fun getAllNodes(node: ASTNode): MutableList<LeafElement?> {
            val result: MutableList<LeafElement?> = ArrayList()
            getNodesRecursive(result, node)
            return result
        }

        private fun getNodesRecursive(result: MutableList<LeafElement?>, node: ASTNode) {
            for (child in node.getChildren(null)) {
                if (child is PsiWhiteSpace || child is PsiComment) {
                    // Skip whitespace
                } else if (child is LeafElement) {
                    result.add(child)
                } else {
                    getNodesRecursive(result, child)
                }
            }
        }
    }
}
