INCLUDE includes.ink

-> Main

=== Main ===
VAR test_string = "Hello, world!"
VAR test_number = 42
~print("label", "Move node 1 by 100, 10.")
~move_object_by("node", 100, 10)
~print("label", "Let's wait for 5 seconds.")
~wait(5)
~print("label", "5 seconds have passed.")
~print("label", "Move node 2 to 0, 0.")
~move_object_to("node2", 0, 0)
~print("label", "The end.")
~destroy_object("node2")
{ get_game_var("test_var1") == "okay":
    ~print("label", "Variable 1 looks okay!")
}
{ get_game_var("test_var2") != "okay":
    ~print("label", "Variable 2 does not look okay!")
}
-> DONE