head(xs)        := xs[0]
tail(xs)        := for{i:int; i>0; xs[i]}
cons(x,xs)      := for{x | xs[i:int]}
snoc(xs,x)      := for{xs[i:int] | x}
append(xs,ys)   := for{xs[i:int] | ys[j:int]}
map(f,xs)       := for(f(xs[i:int]))
