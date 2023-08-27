package com.github.aarcangeli.ridertoys

import com.intellij.lang.ASTNode
import com.intellij.lang.folding.FoldingDescriptor
import com.intellij.openapi.util.TextRange
import com.jetbrains.rider.cpp.fileType.lexer.CppTokenTypes

internal class FoldingDescriptorFinder(private val rootNode: ASTNode) {
    private val result: MutableList<FoldingDescriptor> = ArrayList()

    fun foldingDescriptors(): Array<FoldingDescriptor>  = result.toTypedArray()

    fun findElements() {
        result.clear()
        val iterator = RecursiveNodeIterator(rootNode)
        while (!iterator.eof()) {
            if (iterator.currentElementType() === CppTokenTypes.LBRACE) {
                scanScope(iterator)
                continue
            }
            iterator.advanceElement()
        }
    }

    private fun scanScope(iterator: RecursiveNodeIterator) {
        assert(iterator.currentElementType() === CppTokenTypes.LBRACE)
        iterator.advanceElement()
        var currentBlockStart = -1
        while (!iterator.eof()) {
            if (iterator.currentElementType() === CppTokenTypes.LBRACE) {
                scanScope(iterator)
                continue
            }
            if (iterator.currentElementType() === CppTokenTypes.RBRACE) {
                if (currentBlockStart != -1) {
                    val end = iterator.previousElement!!.textRange.endOffset
                    result.add(FoldingDescriptor(rootNode, TextRange(currentBlockStart, end)))
                }
                iterator.advanceElement()
                break
            }
            if (iterator.currentElementType() === CppTokenTypes.KEYWORD && iterator.nextElementType() === CppTokenTypes.COLON) {
                val text = iterator.currentElement!!.text
                if (text == "public" || text == "private" || text == "protected") {
                    // Create block if necessary
                    if (currentBlockStart != -1) {
                        val end = iterator.previousElement!!.textRange.endOffset
                        result.add(FoldingDescriptor(rootNode, TextRange(currentBlockStart, end)))
                    }
                    iterator.advanceElement()
                    iterator.advanceElement()
                    currentBlockStart = iterator.previousElement!!.textRange.endOffset
                    continue
                }
            }
            iterator.advanceElement()
        }
    }
}
