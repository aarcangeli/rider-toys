package com.github.aarcangeli.ridertools

import com.intellij.lang.ASTNode
import com.intellij.lang.folding.FoldingBuilder
import com.intellij.lang.folding.FoldingDescriptor
import com.intellij.openapi.editor.Document
import com.intellij.openapi.project.DumbAware

class CppFoldBuilder : FoldingBuilder, DumbAware {
    override fun buildFoldRegions(node: ASTNode, document: Document): Array<FoldingDescriptor> {
        val finder = FoldingDescriptorFinder(node)
        finder.findElements()
        return finder.foldingDescriptors()
    }

    override fun getPlaceholderText(node: ASTNode) = null
    override fun isCollapsedByDefault(node: ASTNode) = false
}
