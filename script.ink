INCLUDE includes.ink

-> Main

=== Main ===
~print("label", "Move the node by 100, 10.")
~move_object_by("node", 100, 10)
~print("label", "Let's wait for 5 seconds.")
~wait(5)
~print("label", "5 seconds have passed.")
~print("label", "Move the node to 0, 0.")
~move_object_to("node", 0, 0)
~print("label", "The end.")
-> DONE