//go:build ignore

package main

import (
    "fmt"
    "log"
    "os"
    "net/http"
)

type Page struct {
    Title string
    Body  []byte
}





func viewHandler(w http.ResponseWriter, r *http.Request) {
    title := r.URL.Path[len("/view/"):]
    p, _ := loadPage(title)
    fmt.Fprintf(w, "%s", p.Body)
}

func handler(w http.ResponseWriter, r *http.Request) {
    fmt.Fprintf(w, "Hi there, this a simple http server to view content of the files!", r.URL.Path[1:])
}

func main() {
    http.HandleFunc("/view/", viewHandler) 
    http.HandleFunc("/", handler)
    log.Fatal(http.ListenAndServe(":8080", nil))
}
