package com.github.aarcangeli.ridertoys

import com.jetbrains.rider.settings.simple.SimpleOptionsPage

class TestOptionsPage : SimpleOptionsPage("My Option Name", "RiderToys.TestOptionsPage") {
    override fun getId(): String {
        return pageId
    }
}
