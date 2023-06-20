#detectTile(e) {
    this.#buffer.clear();
    var cursor = Cursor.get();
    var pos = cursor.local().remove(cursor.offset);
    pos.x /= Settings.zoom;
    pos.y /= Settings.zoom;
    if (pos.x < 0 || pos.y < 0 || pos.x > Settings.mapSizeX || pos.y > Settings.mapSizeY) { return; }

    var tiles = this.#generator.getTiles();
    for (let i = 0; i < tiles.length; i++) {
        const tile = tiles[i];
        var points = tile.getVertices();

        if (!tile.isDummy && Collision.polygonPoint(points, pos.x, pos.y)) {
            tile.switchType();
            for (let r = 0; r < points.length; r++) {
                const vc = points[r];
                const vn = r + 1 >= points.length ? points[0] : points[r + 1];

                this.#buffer.stroke(0, 0, 255);
                this.#buffer.strokeWeight(3);
                this.#buffer.line(vc.x, vc.y, vn.x, vn.y);
            }
        }
    }
    this.#generator.redraw();
}