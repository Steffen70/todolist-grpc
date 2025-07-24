from __future__ import annotations

import argparse
from conan.api.conan_api import ConanAPI
from conan.api.model import RecipeReference, PkgReference


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("recipe_ref", help="Full recipe reference incl. hash (e.g. <package>/<version>#<hash>)")
    ap.add_argument("pkg_id", help="The package ID listed under \"packages\" in the \"conan list $RECIPE_REF:*\" output")
    args = ap.parse_args()

    api = ConanAPI()
    full_ref = f"{args.recipe_ref}:{args.pkg_id}"
    pref = PkgReference.loads(full_ref)
    build_path = api.cache.build_path(pref)
    print(build_path)


if __name__ == "__main__":
    main()
