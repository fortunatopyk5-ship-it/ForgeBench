# STATUS

Current objective:
Prove a cutoff/volume-uniform physical-scale clustering estimate along the asymptotically-free Wilson trajectory, or reduce it to a normalization-invariant functional inequality.

Current strongest result:
YM-BRIDGE-1: assuming an OS-reconstructible continuum limit and a common exponential Euclidean-time decay rate m_*>0 on a dense centered gauge-invariant local algebra, the reconstructed Hamiltonian obeys Spec(H)∩(0,m_*)=∅. The proof is a direct spectral-measure argument.

Current blocking lemma:
YM-IR-1 / YM-FI-1. Need a rate in lattice units gamma(a) satisfying gamma(a) >= c a Lambda_YM, uniformly in volume as a->0. A promising sufficient proxy is lambda_a/v_a >= c a Lambda_YM, where lambda_a is a reversible stochastic-quantization Poincare rate and v_a is a local commutator/influence propagation scale.

Assumptions:
- compact simple gauge group G;
- Wilson lattice regularization used for the current route;
- continuum observable renormalization/convergence is separated into YM-UV-1 rather than assumed silently.

Verified:
- finite-cutoff Wilson measure is a genuine finite-dimensional Haar Gibbs measure;
- reflection positivity / positive transfer-matrix machinery is classical for Wilson lattice gauge theory;
- strong-coupling SU(N) functional inequalities and exponential correlation decay are rigorous (Shen–R. Zhu–X. Zhu);
- clustering-to-gap bridge under OS reconstruction is proved in lemmas.md;
- positivity of a lattice gap for each fixed cutoff/volume is not sufficient for the continuum gap.

Unverified:
- full d=4 continuum OS construction on R^4;
- nontriviality of that continuum limit;
- YM-IR-1 at beta->infinity;
- YM-FI-1 at weak coupling;
- uniform thermodynamic-limit control on the continuum trajectory.

Next action:
Attack YM-FI-1 quantitatively. Derive a general covariance-decay theorem from (i) a Poincare inequality for a reversible local semigroup and (ii) a finite-propagation/commutator matrix estimate; then determine the exact scaling requirement on lambda_a/v_a and compare it with a Lambda_YM.


Update — Iteration 1b:
A new abstract bridge YM-SEMIGROUP-1 was proved:
PI relaxation + weighted finite-propagation of local gradients gives a common spatial clustering rate
gamma >= kappa lambda/[2(lambda+v_kappa)].
Thus a sufficient cutoff target is
lambda_a/(lambda_a+v_{kappa,a}) >= c a Lambda_YM.
However, adversarial analysis shows a global PI may be too strong because auxiliary slow global/topological modes need not coincide with the physical local spectrum.

A second, cleaner mass-gap target is YM-RG-LANDING-1:
prove that exact RG lands, at block scale a_*~Lambda_YM^{-1}, inside a cutoff-independent Kotecky-Preiss polymer domain. If this holds, clustering and the physical gap follow while reflection positivity is used only at the original Wilson lattice level.
