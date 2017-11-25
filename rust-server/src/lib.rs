
extern crate walkdir;

use std::collections::hash_map::HashMap;
use std::path::Path;
use std::borrow::Borrow;
use std::rc::Rc;

#[derive(Eq, PartialEq, Hash, Clone, Ord, PartialOrd, Default, Debug)]
pub struct CountedString(pub Rc<String>);

impl From<String> for CountedString {
    fn from(value: String) -> Self {
        CountedString(Rc::new(value))
    }
}

impl From<Rc<String>> for CountedString {
    fn from(value: Rc<String>) -> Self {
        CountedString(value)
    }
}

impl<'a> From<&'a Rc<String>> for CountedString {
    fn from(value: &'a Rc<String>) -> Self {
        CountedString(value.clone())
    }
}

impl Borrow<str> for CountedString {
    fn borrow(&self) -> &str {
        &(&*self.0)
    }
}

impl Borrow<String> for CountedString {
    fn borrow(&self) -> &String {
        &self.0
    }
}

impl std::fmt::Display for CountedString {
    fn fmt(&self, f: &mut std::fmt::Formatter) -> std::fmt::Result {
        (&self.0 as &std::fmt::Display).fmt(f)
    }
}

pub struct IterContentsDebug<I>(pub I);

macro_rules! impl_subformat {
    ($string:expr) => {
        fn fmt(&self, f: &mut std::fmt::Formatter) -> std::fmt::Result {
            let mut state = f.debug_list();
            for t in (self.0)() {
                state.entry(&format_args!($string, t));
            }
            state.finish()
        }
    };
}

impl<T: std::fmt::Debug + Sized, F, I> std::fmt::Debug for IterContentsDebug<F>
    where
        F: Fn() -> I,
        I: std::iter::Iterator<Item=T>,
{
    impl_subformat!("{:?}");
}

impl<T: std::fmt::Debug + Sized + std::fmt::LowerHex, F, I> std::fmt::LowerHex for IterContentsDebug<F>
    where
        F: Fn() -> I,
        I: std::iter::Iterator<Item=T>,
{
    impl_subformat!("{:x}");
}

impl<T: std::fmt::Debug + Sized + std::fmt::UpperHex, F, I> std::fmt::UpperHex for IterContentsDebug<F>
    where
        F: Fn() -> I,
        I: std::iter::Iterator<Item=T>,
{
    impl_subformat!("{:X}");
}

pub struct Directory(HashMap<String, (&'static str, Box<[u8]>)>);

impl Directory {
    pub fn new(path: &Path) -> Self {
        let mut directory = HashMap::new();
        let walker = walkdir::WalkDir::new(path)
            .follow_links(true)
            .into_iter()
            .filter_entry(
                |f|
                    f.file_name().to_str().map(|s| !s.starts_with(".")).unwrap_or(true)
            )
        ;
        for result in walker {
            let entry = match result {
                Ok(entry) => entry,
                Err(e) => panic!("Failed to read directory {:?}: {:?}", path, e),
            };
            let f = entry.path();
            if !f.is_file() { continue }

            let (file, contents) = match f.strip_prefix(path) {
                Ok(relative) => {
                    let mut buf = Default::default();
                    use std::io::Read;
                    match std::fs::File::open(f) {
                        Ok(mut file) => match file.read_to_end(&mut buf) {
                            Ok(_) => (relative, buf.into_boxed_slice()),
                            Err(e) => {
                                println!("Error reading {:?}: {:?}", f, e);
                                continue
                            },
                        },
                        Err(e) => {
                            println!("Error opening {:?}: {:?}", f, e);
                            continue
                        },
                    }
                },
                Err(e) => {
                    println!("Error stripping {:?} prefix {:?}: {:?}", f, path, e);
                    continue
                },
            };
            let mut name =
                if let Some(name) = file.to_str() { name }
                else { continue }
                .replace(std::path::MAIN_SEPARATOR, "/");
            name.insert(0, '/');
            let extension = file.extension().and_then(std::ffi::OsStr::to_str).map(str::to_lowercase);
            let extension = extension.as_ref().map(String::as_str);
            let content_type = match extension {
                Some("html") |
                Some("htm")
                    => "text/html; charset=utf-8",
                Some("js")
                    => "text/javascript; charset=utf-8",
                Some("css")
                    => "text/css; charset=utf-8",
                Some("png")
                    => "image/png",
                Some("json")
                    => "application/json",
                Some("ico")
                    => "image/x-icon",
                _ => "application/octet-stream",
            };
            use std::collections::hash_map::Entry::*;
            match directory.entry(name) {
                Occupied(entry) => panic!("Duplicate file {:?}", entry.key()),
                Vacant(entry) => { entry.insert((content_type, contents)); },
            }
        }

        #[cfg(debug_assertions)]
        println!("Done reading: {:?}", IterContentsDebug(|| directory.keys()));

        Directory(directory)
    }

    pub fn get(&self, token: &str) -> Option<(&'static str, &[u8])> {
        self.0.get(token).map(|&(ct, ref d)| (ct, &*d as &[u8]))
    }
}

#[cfg(test)]
mod tests {
    #[test]
    fn iter_contents_debug() {
        use super::IterContentsDebug;
        let data = &[1, 2, 3];
        assert_eq!("[1, 2, 3]", format!("{:?}", IterContentsDebug(|| data.iter())))
    }
}
