package com.github.aarcangeli.ridertools

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class TestOptionsPage : SimpleOptionsPage("My Option Name", "RiderTools.TestOptionsPage") {
    override fun getId(): String {
        return pageId
    }
}
